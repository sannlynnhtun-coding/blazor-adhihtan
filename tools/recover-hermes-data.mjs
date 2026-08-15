import fs from "node:fs";
import path from "node:path";
import vm from "node:vm";

const [inputPath, outputPath] = process.argv.slice(2);

if (!inputPath || !outputPath) {
  console.error("Usage: node tools/recover-hermes-data.mjs <decompiled.js> <output.json>");
  process.exit(1);
}

const source = fs.readFileSync(inputPath, "utf8");

function getModuleSource(moduleId) {
  const marker = `// === Module ${moduleId}:`;
  const start = source.indexOf(marker);
  if (start < 0) {
    throw new Error(`Module ${moduleId} was not found.`);
  }

  const next = source.indexOf("// === Module ", start + marker.length);
  return source.slice(start, next < 0 ? source.length : next);
}

function evaluateDefaultExport(moduleId) {
  const moduleSource = getModuleSource(moduleId);
  const executable = moduleSource
    .replace(/^\/\/.*$/gm, "")
    .replace(/export default ([^;]+);\s*$/m, "globalThis.__result = $1;");

  const context = { __result: undefined };
  vm.runInNewContext(executable, context, {
    filename: `hermes-module-${moduleId}.js`,
    timeout: 2_000,
  });

  return structuredClone(context.__result);
}

function evaluateLocalizationMap() {
  const moduleSource = getModuleSource(542);
  const match = moduleSource.match(/const obj = (\{[\s\S]*?\});\s*\n\s*export const DATE_LOCALIZATION/);
  if (!match) {
    throw new Error("The localized spell-name table was not found in module 542.");
  }

  const context = { __result: undefined };
  vm.runInNewContext(`globalThis.__result = ${match[1]};`, context, {
    filename: "hermes-module-542-localization.js",
    timeout: 2_000,
  });
  return structuredClone(context.__result);
}

const localizedSpellNames = evaluateLocalizationMap();
const categories = evaluateDefaultExport(524);
const schedules = {
  "1": evaluateDefaultExport(544),
  "2": evaluateDefaultExport(545),
  "3": evaluateDefaultExport(546),
  "4": evaluateDefaultExport(547),
};

for (const levels of Object.values(schedules)) {
  for (const level of levels) {
    for (const spell of level.datasources) {
      spell.spellName = localizedSpellNames[spell.spellName] ?? spell.spellName;
    }
  }
}

const recovered = {
  schemaVersion: 1,
  source: {
    packageName: "com.a_dhi_htan",
    appVersion: "1.3.1",
    hermesBytecodeVersion: 96,
  },
  categories,
  schedules,
  localizedSpellNames,
};

fs.mkdirSync(path.dirname(outputPath), { recursive: true });
fs.writeFileSync(outputPath, `${JSON.stringify(recovered, null, 2)}\n`, "utf8");
console.log(`Recovered ${categories.length} categories and ${Object.values(schedules).flat().length} schedule levels.`);
