import fs from "node:fs";
import vm from "node:vm";

const [, , inputPath, outputPath] = process.argv;
if (!inputPath || !outputPath) {
    console.error("Usage: node recover-hermes-font-converter.mjs <app-modules.js> <output.js>");
    process.exit(1);
}

const source = fs.readFileSync(inputPath, "utf8");
const moduleStart = source.indexOf("// === Module 489: replace_with_rule ===");
const moduleEnd = source.indexOf("// === Module 490:", moduleStart);
if (moduleStart < 0 || moduleEnd < 0) {
    throw new Error("Hermes module 489 (rabbit-node rules) was not found.");
}

const moduleSource = source.slice(moduleStart, moduleEnd);

function extractRules(methodName) {
    const methodStart = moduleSource.indexOf(`${methodName}(`);
    const declarationStart = moduleSource.indexOf("const items =", methodStart);
    const arrayStart = moduleSource.indexOf("[", declarationStart);
    if (methodStart < 0 || declarationStart < 0 || arrayStart < 0) {
        throw new Error(`Could not locate ${methodName} rules.`);
    }

    let depth = 0;
    let quote = null;
    let escaped = false;
    for (let index = arrayStart; index < moduleSource.length; index += 1) {
        const character = moduleSource[index];
        if (quote) {
            if (escaped) escaped = false;
            else if (character === "\\") escaped = true;
            else if (character === quote) quote = null;
            continue;
        }

        if (character === "\"" || character === "'" || character === "`") {
            quote = character;
        } else if (character === "[") {
            depth += 1;
        } else if (character === "]") {
            depth -= 1;
            if (depth === 0) {
                return vm.runInNewContext(`(${moduleSource.slice(arrayStart, index + 1)})`);
            }
        }
    }

    throw new Error(`The ${methodName} rule array was incomplete.`);
}

const zg2uniRules = extractRules("zg2uni");
const uni2zgRules = extractRules("uni2zg");
const output = `// Generated from APK Hermes module 489 (rabbit-node 1.0.4).\n` +
`(() => {\n` +
`    const zg2uniRules = ${JSON.stringify(zg2uniRules)};\n` +
`    const uni2zgRules = ${JSON.stringify(uni2zgRules)};\n` +
`    const myanmarPattern = /[\\u1000-\\u109f\\uaa60-\\uaa7f\\ua9e0-\\ua9ff]/;\n` +
`    const applyRules = (rules, value) => rules.reduce((text, rule) => text.replace(new RegExp(rule.from, "g"), rule.to), value);\n` +
`    const originalText = new WeakMap();\n` +
`    let encoding = "unicode";\n` +
`\n` +
`    const transformTextNode = node => {\n` +
`        const record = originalText.get(node);\n` +
`        if (encoding === "zawgyi") {\n` +
`            if (record?.rendered === node.data || !myanmarPattern.test(node.data)) return;\n` +
`            const source = node.data;\n` +
`            const rendered = applyRules(uni2zgRules, source);\n` +
`            originalText.set(node, { source, rendered });\n` +
`            if (rendered !== source) node.data = rendered;\n` +
`        } else if (record) {\n` +
`            if (node.data === record.rendered) node.data = record.source;\n` +
`            originalText.delete(node);\n` +
`        }\n` +
`    };\n` +
`\n` +
`    const transformTree = root => {\n` +
`        if (!root) return;\n` +
`        if (root.nodeType === Node.TEXT_NODE) {\n` +
`            transformTextNode(root);\n` +
`            return;\n` +
`        }\n` +
`        const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);\n` +
`        while (walker.nextNode()) transformTextNode(walker.currentNode);\n` +
`    };\n` +
`\n` +
`    const observer = new MutationObserver(records => {\n` +
`        for (const record of records) {\n` +
`            if (record.type === "characterData") transformTextNode(record.target);\n` +
`            for (const node of record.addedNodes ?? []) transformTree(node);\n` +
`        }\n` +
`    });\n` +
`\n` +
`    const startObserver = () => {\n` +
`        if (!document.body) return;\n` +
`        observer.disconnect();\n` +
`        observer.observe(document.body, { subtree: true, childList: true, characterData: true });\n` +
`    };\n` +
`\n` +
`    window.adhihtanMyanmar = {\n` +
`        unicodeToZawgyi: value => applyRules(uni2zgRules, value ?? ""),\n` +
`        zawgyiToUnicode: value => applyRules(zg2uniRules, value ?? ""),\n` +
`        setEncoding: value => {\n` +
`            encoding = value === "zawgyi" ? "zawgyi" : "unicode";\n` +
`            transformTree(document.body);\n` +
`            startObserver();\n` +
`        }\n` +
`    };\n` +
`    window.addEventListener("DOMContentLoaded", startObserver, { once: true });\n` +
`})();\n`;

fs.writeFileSync(outputPath, output, "utf8");
console.log(`Recovered ${zg2uniRules.length} Zawgyi→Unicode and ${uni2zgRules.length} Unicode→Zawgyi rules.`);
