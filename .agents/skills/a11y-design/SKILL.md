---
name: a11y-design
description: Accessibility design guidance for web pages, apps, and UI components. Use when creating or modifying pages, layouts, forms, navigation, modals, interactive controls, visual states, responsive UI, Tailwind CSS/daisyUI/Preline/FlyonUI dashboard interfaces, Cruip/Mosaic-style SaaS dashboards, Stablo-style blogs, landing pages, ecommerce stores, documentation sites, user research, information architecture, user flows, wireframes, prototypes, usability testing, developer handoff, design systems, inclusive user journeys, device-agnostic experiences, dynamic content, or when asked to review/suggest accessible improvements against WCAG 2.2 AA, POUR, keyboard support, semantics, focus, contrast, labels, errors, and reduced motion.
---

# A11y Design

## Intro

Use this skill to shape UI from idea to implementation without treating accessibility as cleanup. Start with the user's goal and page type, define landmarks and content hierarchy, choose native or existing design-system components, then tune visual style, responsive behavior, theme, and motion after the core flow works.

For Tailwind CSS/daisyUI/Preline-style work, compose pages from semantic component classes and use utilities for layout, spacing, and customization. Component classes speed up design, but still verify the actual HTML, labels, focus behavior, states, contrast, and keyboard paths.

Common page flow patterns:

- **Dashboard**: sidebar/topbar, command search, icon actions, page title, compact metric cards, charts, filters, tables, import/export, loading/empty/error states. For Preline-style admin dashboards, keep the shell clean, roomy, themeable, and data-dense without hiding labels, focus states, table headers, or chart alternatives.
- **Blog/content**: nav, author/brand intro, categories, article cards, featured/top articles, newsletter, footer.
- **Landing page**: hero with primary CTA, proof metrics/logos, feature sections, steps, testimonials, pricing/FAQ, footer.
- **Ecommerce**: browse/filter/sort, product cards, product detail gallery, variants, cart/checkout path, related products.
- **Documentation**: home, install/get-started path, sidebar, on-page links, search, code demos, theme switch.

Template style cues:

- **FlyonUI dashboard**: dense admin app shell with nested sidebar groups, top search/command access, theme/language controls, notification/activity surfaces, profile menus, KPI cards, charts, schedules, country/payment tables, and ecommerce store switching.
- **Cruip Mosaic dashboard**: SaaS analytics workspace with collapsible sidebar, search modal, help/notification/profile menus, dark-light switch, filters, add-view actions, compact metric cards, and data visualizations.
- **Stablo blog**: editorial blog layout with clean nav, category/tag browsing, featured/latest article cards, thumbnails, author/date metadata, excerpts, and newsletter/footer areas.

## Working Style

Treat accessibility as part of the design change, not a separate pass. Preserve the product's existing visual language and suggest the smallest changes that improve real use.

When creating or modifying UI:

1. Reuse existing accessible components and patterns first.
2. Prefer native HTML semantics before ARIA.
3. Keep all interactive behavior reachable and understandable by keyboard.
4. Keep visible focus states clear and not color-only.
5. Target WCAG 2.2 AA contrast unless the project states another requirement.
6. Give every form control a visible label, helpful hint, and clear error path.
7. Keep heading order, landmarks, and page titles meaningful.
8. Give icon-only controls an accessible name.
9. Use meaningful alt text for informative images and empty alt for decorative images.
10. Respect reduced-motion settings for animations, parallax, and auto-playing effects.
11. Check responsive layouts for text clipping, overlap, reading order, and zoom resilience.
12. Add short user-facing a11y notes when there are meaningful tradeoffs or suggestions.

## UX Process Notes

Use POUR as the quick framing question: can users perceive the information, operate the controls, understand the flow, and rely on it across browsers, devices, and assistive technologies?

When work touches product UX beyond a single visual tweak, suggest the smallest relevant improvements from these areas:

- **UI/UX fundamentals**: Start from user research, user goals, content hierarchy, information architecture, navigation, core flows, and reusable UI components. Carry accessibility into wireframes, prototypes, usability tests, iteration, and developer handoff specs: labels, focus order, states, errors, responsive behavior, and keyboard paths.
- **Design systems**: Encode accessibility in component docs and tokens: keyboard behavior, focus rules, ARIA needs, screen reader expectations, contrast-safe color pairs, spacing, and target sizes.
- **Inclusive journeys**: Map flows for screen reader users, keyboard-only users, low-vision/magnified browsing, cognitive load, different tech familiarity, and temporary or situational constraints.
- **Aesthetics**: Start with semantic content and operable controls, then layer animation, decorative effects, and brand expression only when they do not block use. Use shape, text, iconography, pattern, or texture alongside color.
- **Testing**: Combine automated checks with manual keyboard, screen reader, form-error, zoom/reflow, and cross-device checks. For high-impact flows, recommend feedback from people who use assistive technology regularly.
- **Device-agnostic use**: Check iOS VoiceOver, Android TalkBack, desktop screen readers, touch, stylus, voice control, switch input, hover alternatives, essential-content loading, and usable controls when magnified.
- **Dynamic content**: For SPAs, modals, overlays, live regions, and custom widgets, restore focus, announce meaningful changes, keep ARIA state current, avoid redundant labels, and do not override native semantics.

Source inspiration: [A11Y Collective, "Accessible UX Design: 6 Advanced Techniques to Elevate Your Process"](https://www.a11y-collective.com/blog/accessible-ux-design/).
Source inspiration: [Medium, "UI/UX Design 101: A Comprehensive Guide for Beginners"](https://medium.com/@nile.bits/ui-ux-design-101-a-comprehensive-guide-for-beginners-f6588c86e963).
Source inspiration: [Dribbble, "UX Design 101: A Comprehensive Guide"](https://dribbble.com/resources/education/ux-design).
Source inspiration: [daisyUI introduction](https://daisyui.com/docs/intro/), [Nexus ecommerce dashboard](https://nexus.daisyui.com/dashboards/ecommerce), [blog template](https://blog-template.daisyui.com/), [mobile landing](https://mobile-landing.daisyui.com/), [SaaS landing](https://saas-landing.daisyui.com/), [online store template](https://daisyui.com/store/online-store/), and [documentation template](https://daisyui.com/store/documentation-template/).
Source inspiration: [Preline UI admin dashboard template](https://preline.co/templates/dashboards/admin-dashboard/).
Source inspiration: [FlyonUI free dashboard](https://demos.flyonui.com/templates/html/dashboard-free/), [FlyonUI default dashboard](https://demos.flyonui.com/templates/html/dashboard-default/), [FlyonUI ecommerce dashboard](https://demos.flyonui.com/templates/html/dashboard-ecommerce/), [Cruip Mosaic dashboard](https://preview.cruip.com/mosaic/index.html), and [Stablo blog template](https://stablo-pro.web3templates.com/).

## Checklist

### Structure

- Use one main content area per page.
- Keep headings hierarchical; do not choose heading levels only for visual size.
- Use buttons for actions and links for navigation.
- Use lists, tables, fieldsets, and labels when the content is actually those things.

### Keyboard And Focus

- Tab order should follow the visual and reading order.
- Custom controls must support Enter/Space and arrow keys when expected.
- Dialogs, menus, popovers, and drawers must manage focus on open and return focus on close.
- Never remove outlines without replacing them with an equally visible focus style.

### Forms

- Labels should remain available after input.
- Error text should be programmatically connected to the field when the framework supports it.
- Required fields need visible indication and validation messages.
- Prefer native input types such as `email`, `date`, `number`, `search`, and `tel`.

### Visual Design

- Text, icons that carry meaning, focus rings, chart marks, and controls need sufficient contrast.
- Do not rely on color alone for status, errors, selection, or chart meaning.
- Touch targets should be comfortable; avoid tiny icon-only hit areas.
- Text must not overlap, clip, or shrink with viewport-width font hacks.

### Motion And Media

- Pause or avoid auto-playing motion when it distracts from the task.
- Respect `prefers-reduced-motion`.
- Captions/transcripts are needed for user-facing media when applicable.
- Avoid flashing patterns.

## Implementation Bias

Use the platform before adding code:

- Native controls over custom widgets.
- CSS states over JavaScript state when possible.
- Existing component props over new wrapper components.
- Existing test tooling over new dependencies.

ARIA is for filling semantic gaps. If native HTML already supplies the role, state, name, and behavior, use native HTML.

## Verification

For UI code changes, run the smallest available check that would catch the likely failure:

- Existing a11y lint or test command if the repo has one.
- Existing Playwright/Cypress flow for keyboard and focus behavior.
- Manual smoke check for tab order, focus visibility, labels, contrast, zoom, and mobile layout.

Do not add a new accessibility dependency for a one-off review unless the user asks or the project already uses that toolchain.
