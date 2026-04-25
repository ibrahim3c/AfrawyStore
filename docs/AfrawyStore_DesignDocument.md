# ◈ AFRAWY STORE — Design Document
### System UI/UX & Visual Design Specification · v1.0

---

> *"A tool in the right hand is worth a thousand words — a screen in the right design is worth a thousand clicks."*

---

## ◈ Table of Contents

1. [Design Philosophy](#1--design-philosophy)
2. [Brand Identity](#2--brand-identity)
3. [Color System](#3--color-system)
4. [Typography](#4--typography)
5. [Layout Architecture](#5--layout-architecture)
6. [Component Library](#6--component-library)
7. [Page-by-Page Design Specs](#7--page-by-page-design-specs)
8. [Data Visualization Design](#8--data-visualization-design)
9. [Responsive & RTL Design](#9--responsive--rtl-design)
10. [Motion & Micro-Interactions](#10--motion--micro-interactions)
11. [Accessibility Standards](#11--accessibility-standards)
12. [Design Tokens Reference](#12--design-tokens-reference)

---

## 1 ◈ Design Philosophy

AFRAWY STORE is not just a management system — it is the **digital nervous system** of a physical store. Every interaction should feel grounded, fast, and trustworthy. The design language is built around three pillars:

```
┌────────────────────────────────────────────────────────────┐
│                                                            │
│   CLARITY      ·    Clear hierarchy. Zero confusion.       │
│   EFFICIENCY   ·    Fewest clicks to complete any task.    │
│   TRUST        ·    Data feels safe, accurate, and real.   │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

The aesthetic direction is **Industrial Precision** — drawing from the physical world of tools, steel, and paint: structured grids, bold weight, warm neutrals contrasted with sharp accent colors. It honors the Arabic RTL reading flow naturally, not as an afterthought.

---

## 2 ◈ Brand Identity

### 2.1 Logo Concept

The **AFRAWY STORE** logo mark combines two visual elements:

```
  ◈  A  F  R  A  W  Y
     ─────────────────
     STORE  ·  متجر الأفراوي
```

- **Icon:** A geometric diamond `◈` — symbolizing precision and value.
- **Wordmark:** The name **AFRAWY** in bold uppercase, paired with a hairline rule and the Arabic subtitle.
- **Tagline:** *"متجر الأدوات والدهانات"*

### 2.2 Brand Personality Keywords

| Arabic | English |
|--------|---------|
| موثوق | Trustworthy |
| منظّم | Organized |
| محلّي | Local |
| دقيق | Precise |
| عملي | Practical |

---

## 3 ◈ Color System

The palette is drawn from the physical store: the warmth of wood, the coldness of steel, and the vividness of paint.

### 3.1 Core Palette

| Token | Hex | Usage | Preview |
|-------|-----|-------|---------|
| `--color-primary` | `#1A3C5E` | Primary actions, links, brand accent | ████ Deep Navy |
| `--color-primary-light` | `#2C5F8A` | Hover states, card borders | ████ Steel Blue |
| `--color-accent` | `#E07B2C` | CTAs, highlights, badges | ████ Copper Orange |
| `--color-accent-soft` | `#F5E0CC` | Accent backgrounds, tags | ████ Pale Copper |
| `--color-surface` | `#F7F5F2` | Page background | ████ Warm White |
| `--color-surface-2` | `#EDEAE6` | Card backgrounds, table rows | ████ Linen |
| `--color-ink` | `#1E1E1E` | Primary text | ████ Near Black |
| `--color-ink-muted` | `#6B6560` | Secondary text, labels | ████ Warm Gray |
| `--color-border` | `#D4CFC8` | Dividers, input borders | ████ Stone |

### 3.2 Status Color System

Critical for inventory alerts and sale statuses:

| Status | Token | Hex | Description |
|--------|-------|-----|-------------|
| 🟢 Good | `--status-good` | `#2D7A4F` | Stock above threshold |
| 🟡 Warning | `--status-warn` | `#B5860D` | Stock at threshold |
| 🔴 Critical | `--status-danger` | `#B83232` | Stock below threshold |
| ⚪ Neutral | `--status-neutral` | `#6B6560` | Voided / Inactive |
| 🔵 Info | `--status-info` | `#1A3C5E` | General informational |

### 3.3 Color Usage Rules

```
DO ✅
  · Use --color-accent ONLY for the single most important action per page.
  · Use status colors ONLY to communicate data states (never for decoration).
  · Maintain a minimum contrast ratio of 4.5:1 for all body text.

DON'T ❌
  · Never use more than 3 colors in a single component.
  · Never use red/green status colors as decorative accents.
  · Never place accent color on primary color backgrounds.
```

---

## 4 ◈ Typography

The interface is entirely in **Arabic (RTL)** using the **Cairo** typeface — a geometric, contemporary Arabic font with excellent legibility at small sizes and strong personality at large sizes.

### 4.1 Type Scale

```css
/* Base: 16px = 1rem */

--type-display:   2.25rem  / 700  →  Dashboard headings, page heroes
--type-heading-1: 1.75rem  / 700  →  Section titles
--type-heading-2: 1.375rem / 600  →  Card headings, modal titles
--type-heading-3: 1.125rem / 600  →  Table headers, sidebar labels
--type-body:      1rem     / 400  →  Primary body text
--type-body-sm:   0.875rem / 400  →  Secondary text, meta data
--type-caption:   0.75rem  / 400  →  Timestamps, footnotes, badges
--type-label:     0.75rem  / 600  →  Form labels (UPPERCASE tracking)
```

### 4.2 Font Loading

```html
<!-- In <head> — loaded before first paint -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link
  href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap"
  rel="stylesheet"
>
<style>
  :root { font-family: 'Cairo', system-ui, sans-serif; }
</style>
```

### 4.3 Numeric Display

For currency and quantities, use **tabular numerals** to keep columns aligned:

```css
.numeric {
  font-variant-numeric: tabular-nums;
  letter-spacing: 0.02em;
}
```

---

## 5 ◈ Layout Architecture

### 5.1 Shell Structure (RTL)

```
┌─────────────────────────────────────────────────────────────────┐
│  ▸ TOP NAV BAR  [height: 60px]                                  │
│    [◈ AFRAWY]     [Search...]     [🔔 3]  [Ahmed ▾]  [خروج]    │
├──────────────┬──────────────────────────────────────────────────┤
│              │                                                  │
│  RIGHT       │  MAIN CONTENT AREA                               │
│  SIDEBAR     │                                                  │
│  [220px]     │  [breadcrumb trail]                              │
│              │  ────────────────                                │
│  ● لوحة      │  [Page Title]                                    │
│    التحكم    │                                                  │
│  ○ المنتجات  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐           │
│  ○ الفئات    │  │      │ │      │ │      │ │  🔴  │           │
│  ○ المخزون   │  │ Card │ │ Card │ │ Card │ │Alert!│           │
│  ○ المبيعات  │  └──────┘ └──────┘ └──────┘ └──────┘           │
│  ○ التقارير  │                                                  │
│  ● المستخدم  │  [  Content Grid / Table / Form  ]               │
│    (Admin)   │                                                  │
│              │                                                  │
├──────────────┴──────────────────────────────────────────────────┤
│  FOOTER  ·  AFRAWY STORE v1.1  ·  للاستخدام الداخلي فقط         │
└─────────────────────────────────────────────────────────────────┘
```

### 5.2 Grid System

- **Content grid:** 12-column, 24px gutters
- **Content max-width:** 1280px (fluid within)
- **Sidebar:** Fixed 220px, collapses to icon-only at <1024px
- **Top nav:** Fixed position, `z-index: 1000`

### 5.3 Spacing Scale

Following an 8-point base grid:

| Token | Value | Usage |
|-------|-------|-------|
| `--space-1` | 4px | Inline micro gaps |
| `--space-2` | 8px | Component inner padding |
| `--space-3` | 12px | Tight component gaps |
| `--space-4` | 16px | Standard padding |
| `--space-5` | 24px | Card padding |
| `--space-6` | 32px | Section gaps |
| `--space-7` | 48px | Major section breaks |
| `--space-8` | 64px | Page-level vertical rhythm |

---

## 6 ◈ Component Library

### 6.1 Cards — Summary Cards (Dashboard)

```
┌────────────────────────────────┐
│  ┌──────┐                      │
│  │  🎨  │  إجمالي المنتجات     │
│  └──────┘  ───────────────     │
│            248                 │
│            ↑ +3 هذا الأسبوع   │
└────────────────────────────────┘
```

**Specs:**
- Background: `--color-surface-2`
- Border: 1px solid `--color-border`, radius `12px`
- Icon container: 48×48px, background `--color-accent-soft`, radius `10px`
- Number: `--type-display`, color `--color-ink`
- Label: `--type-heading-3`, color `--color-ink-muted`
- Trend indicator: colored arrow + small text, `--type-caption`
- Hover: subtle lift `transform: translateY(-2px)`, shadow deepens

### 6.2 Alert Card — Low Stock Warning

```
┌────────────────────────────────────────────────────┐
│  🔴  تنبيه: 7 منتجات وصلت لحد إعادة الطلب         │
│       ──────────────────────────────────           │
│       [ عرض المنتجات ]  [ تجاهل مؤقتاً ]          │
└────────────────────────────────────────────────────┘
```

**Specs:**
- Left border: 4px solid `--status-danger`
- Background: `#FFF0F0`
- Icon: animated pulse ring in `--status-danger`
- CTA button links to `/Inventory?filter=low`

### 6.3 Data Tables

```
┌──────┬──────────────────┬──────────┬──────────┬──────────┬─────────┐
│  ☐   │  اسم المنتج      │   SKU    │ السعر    │ المخزون  │ الحالة │
├──────┼──────────────────┼──────────┼──────────┼──────────┼─────────┤
│  ☑   │  دهان مائي أبيض  │ PT-001   │ 85.00 ج  │  12 لتر  │ ● نشط  │
│  ☐   │  مفتاح ربط 14mm  │ TL-047   │ 22.50 ج  │   2 قطعة │ 🔴 منخفض│
└──────┴──────────────────┴──────────┴──────────┴──────────┴─────────┘
```

**Specs:**
- Header: `--color-surface-2`, text uppercase, `--type-label`, `--color-ink-muted`
- Row height: 52px
- Alternating rows: white / `--color-surface`
- Hover row: `--color-accent-soft` background
- Selected row: left border 3px `--color-primary`
- Status badges: pill shape, colored by status system
- Actions (edit/delete): icon buttons, appear on row hover

### 6.4 Buttons

| Variant | Style | Usage |
|---------|-------|-------|
| **Primary** | `--color-primary` fill, white text | Main CTA (one per page) |
| **Secondary** | White fill, `--color-primary` border | Secondary actions |
| **Danger** | `--status-danger` fill | Delete, Void |
| **Ghost** | No fill, no border, `--color-primary` text | Tertiary actions |
| **Icon-only** | Square, `--color-surface-2` bg | Table row actions |

All buttons: `border-radius: 8px`, `font-weight: 600`, `transition: all 0.15s`

### 6.5 Form Inputs

```
  اسم المنتج *
  ┌──────────────────────────────────────────────────┐
  │  دهان مائي أبيض مات                              │
  └──────────────────────────────────────────────────┘
  ✓ حقل صالح                          [0 / 150 حرف]
```

**Specs:**
- Label: `--type-label` uppercase, `--color-ink-muted`, 4px gap above input
- Input: height 44px, border `--color-border`, radius `8px`
- Focus: border `--color-primary`, box-shadow `0 0 0 3px rgba(26,60,94,0.15)`
- Error: border `--status-danger`, error message below in red
- Valid: subtle checkmark icon in green

### 6.6 Sidebar Navigation

```
│  ─────────────────────────  │
│  ● لوحة التحكم             │   ← Active: copper left-border + bold text
│  ○ المنتجات                │
│    └ إضافة منتج            │   ← Sub-item: indented, lighter
│  ○ الفئات                  │
│  ○ المخزون      ● 7        │   ← Badge: low-stock count
│  ○ المبيعات                │
│  ○ التقارير                │
│  ─────────────────────────  │
│  ○ المستخدمون (Admin)      │
│  ─────────────────────────  │
```

**Specs:**
- Background: `--color-primary` (deep navy)
- Text: white at 80% opacity
- Active item: white 100%, right border 3px `--color-accent`
- Badges: `--color-accent` pill on the left side of item

### 6.7 Status Badges

| State | Arabic Label | Style |
|-------|-------------|-------|
| فوق الحد | ●  جيد | Green pill |
| عند الحد | ● تحذير | Yellow pill |
| أقل من الحد | ● حرج | Red pill, pulsing |
| غير نشط | ○ غير نشط | Gray pill |
| مكتمل | ✓ مكتمل | Green pill |
| ملغي | ✕ ملغي | Red strikethrough style |

---

## 7 ◈ Page-by-Page Design Specs

### Page 1 — Login · `/Account/Login`

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│         [Geometric diagonal background pattern]            │
│              in --color-primary dark tones                  │
│                                                             │
│           ┌───────────────────────────────────┐            │
│           │  ◈  AFRAWY STORE                  │            │
│           │     متجر الأدوات والدهانات         │            │
│           │  ─────────────────────────────    │            │
│           │                                   │            │
│           │  اسم المستخدم                     │            │
│           │  ┌─────────────────────────────┐  │            │
│           │  │                             │  │            │
│           │  └─────────────────────────────┘  │            │
│           │                                   │            │
│           │  كلمة المرور                      │            │
│           │  ┌─────────────────────────────┐  │            │
│           │  │  ••••••••          [ 👁 ]   │  │            │
│           │  └─────────────────────────────┘  │            │
│           │                                   │            │
│           │  ☐  تذكرني                        │            │
│           │                                   │            │
│           │  ┌─────────────────────────────┐  │            │
│           │  │         تسجيل الدخول         │  │            │
│           │  └─────────────────────────────┘  │            │
│           └───────────────────────────────────┘            │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Design notes:**
- Full-bleed background: `--color-primary` with a subtle geometric grid overlay at 10% opacity
- Login card: white, `border-radius: 16px`, deep shadow `0 20px 60px rgba(0,0,0,0.25)`
- Logo icon `◈` animated with a brief fade+scale on page load
- Error state: card shakes horizontally (CSS animation)

---

### Page 2 — Dashboard · `/Dashboard`

**Summary Cards Row (4 across):**

| Card | Icon | Color Accent |
|------|------|-------------|
| إجمالي المنتجات | 🏷️ | `--color-primary-light` |
| إجمالي الفئات | 📂 | `--color-primary-light` |
| مبيعات اليوم | 💰 | `--color-accent` |
| منتجات منخفضة | ⚠️ | `--status-danger` |

**Sales Chart (last 7 days):**
- Chart type: Bar chart with rounded tops
- Bar color: `--color-primary` → gradient to `--color-accent`
- X-axis: Arabic day names (السبت، الأحد...)
- Hover tooltip: Arabic formatted currency

**Latest Transactions Table:**
- Last 10 sales
- Columns: رقم البيع · التاريخ · العناصر · الإجمالي · طريقة الدفع · الحالة
- Status pill for each row

**Low-Stock Panel (bottom):**
- Red-tinted background section
- Product name + current stock + minimum threshold
- Quick action: "+ إضافة مخزون" per row

---

### Page 3 — Products List · `/Products`

**Filter Bar:**
```
[ 🔍 بحث بالاسم أو SKU... ]  [ الفئة ▾ ]  [ الحالة ▾ ]  [ + إضافة منتج ]
```

**Table columns:** ☐ · الصورة · الاسم · SKU · الفئة · سعر البيع · المخزون · الحالة · الإجراءات

**Design highlights:**
- Product image thumbnail (40×40px, rounded, placeholder icon if none)
- Stock shown as colored number: green if healthy, red if low
- Row actions: ✏️ تعديل · 👁 عرض · 🗑 حذف (Admin only, appear on hover)
- Bulk action toolbar slides up from bottom when rows are selected

---

### Page 4 — Create / Edit Product · `/Products/Create`

**Two-column form layout:**

```
┌──────────────────────────────┬──────────────────────────┐
│  المعلومات الأساسية           │  السعر والمخزون           │
│  ─────────────────────────   │  ───────────────────────  │
│  الاسم  [______________]     │  سعر التكلفة  [_______]   │
│  SKU    [______________]     │  سعر البيع    [_______]   │
│  الفئة  [  اختر  ▾    ]     │  هامش الربح   [ 35.3% ]   │
│  الوحدة [______________]     │  ───────────────────────  │
│  الوصف  [______________]     │  صورة المنتج              │
│          [______________]     │  ┌──────────────────────┐ │
│          [______________]     │  │   اسحب أو اختر صورة  │ │
│  حالة نشط [  ◉ نشط    ]     │  └──────────────────────┘ │
└──────────────────────────────┴──────────────────────────┘
                 [ حفظ المنتج ]   [ إلغاء ]
```

**Live profit margin indicator:**
- Small inline display below selling price
- Color codes: `>30%` green, `15-30%` yellow, `<15%` red
- Updates in real-time as user types

---

### Page 10 — New Sale (Point of Sale) · `/Sales/New`

This is the most critical and frequently used page. Design priority: **speed and clarity**.

```
┌───────────────────────────────────────────────────────────────┐
│                       بيع جديد                                │
├────────────────────────────────┬──────────────────────────────┤
│  البحث عن منتج                 │  ملخص الطلب                  │
│  ┌─────────────────────────┐   │  ─────────────────────────   │
│  │ 🔍 اكتب الاسم أو SKU...│   │                              │
│  └─────────────────────────┘   │  دهان مائي أبيض              │
│                                │  [−] 3 [+]    255.00 ج       │
│  ┌─────┬──────────────────┐    │                              │
│  │[IMG]│ دهان مائي أبيض  │    │  مفتاح ربط 14mm              │
│  │     │ PT-001 · 85.00ج │    │  [−] 1 [+]    22.50 ج        │
│  │     │ المخزون: 12     │    │                              │
│  │     │       [ إضافة ] │    │  ─────────────────────────   │
│  ├─────┼──────────────────┤    │  خصم: [________] ج          │
│  │[IMG]│ دهان زيتي أحمر  │    │                              │
│  │     │ PT-008 · 60.00ج │    │  الإجمالي:  277.50 ج        │
│  │     │ المخزون: 5      │    │                              │
│  │     │       [ إضافة ] │    │  طريقة الدفع:               │
│  └─────┴──────────────────┘    │  [نقدي] [بطاقة] [أخرى]      │
│                                │                              │
│                                │  ┌────────────────────────┐  │
│                                │  │   ✓ إتمام البيع         │  │
│                                │  └────────────────────────┘  │
│                                │  [ × إلغاء ]                 │
└────────────────────────────────┴──────────────────────────────┘
```

**Design highlights:**
- Keyboard shortcut hints subtly shown (e.g., `/ للبحث`, `Enter للإضافة`)
- Product quantity `[−] N [+]` controls are large touch-friendly (44px)
- Total updates with a brief number-count animation
- "إتمام البيع" button turns from `--color-primary` to `--color-accent` when cart is non-empty
- Out-of-stock products shown with a red overlay and "نفذ المخزون" tag, disabled

---

### Page 12 — Reports · `/Reports`

**Tab navigation:**

```
[ 📊 تقرير المبيعات ]  [ 📦 حالة المخزون ]  [ 💰 الأرباح والخسائر ]  [ ⚠️ المنخفض ]
```

**Report toolbar per tab:**
```
التاريخ من: [______] إلى: [______]  [ 🔍 تطبيق الفلتر ]   [ PDF ↓ ]  [ CSV ↓ ]
```

**Design notes:**
- Each tab has a distinct accent strip color so users know which report type they're in
- Summary KPI strip at top of each report (totals, averages)
- Charts use consistent color language from the main palette

---

## 8 ◈ Data Visualization Design

### 8.1 Dashboard Sales Chart

```
مبيعات آخر 7 أيام
─────────────────────────────────────────────────────

  2400 ج │         ████
  2000 ج │    ████ ████ ████
  1600 ج │    ████ ████ ████ ████
  1200 ج │ ██ ████ ████ ████ ████ ██
   800 ج │ ██ ████ ████ ████ ████ ████ ██
        └──────────────────────────────────────
           الجمعة  السبت  الأحد  الإثنين  الثلاثاء  الأربعاء  الخميس
```

**Chart.js configuration principles:**
- Font: `'Cairo'` for all labels
- Color: bars use `--color-primary` to `--color-primary-light` gradient
- Today's bar: highlighted with `--color-accent`
- Tooltip: Arabic-formatted, shows total sales + profit for that day
- Rounded bar tops: `borderRadius: 6`
- Animation: bars grow from bottom on page load (`duration: 800ms, easing: 'easeOutQuart'`)

### 8.2 Inventory Status Distribution (Reports)

Donut chart showing percentage of products in each stock state:

- Green segment: above threshold
- Yellow segment: at threshold
- Red segment: below threshold
- Center text: total active products count

### 8.3 Profit Trend Line (Reports)

- Area chart with gradient fill
- X-axis: date range
- Line color: `--color-accent`
- Fill: gradient from `--color-accent` to transparent

---

## 9 ◈ Responsive & RTL Design

### 9.1 RTL Implementation

```html
<!-- Applied at the root level — never override -->
<html lang="ar" dir="rtl">

<!-- Bootstrap RTL — must be the first CSS loaded -->
<link rel="stylesheet" href="/css/bootstrap.rtl.min.css">
```

**RTL design checklist:**

| Element | RTL Behavior |
|---------|-------------|
| Sidebar | Fixed on the **right** side |
| Text alignment | `text-align: right` (default via dir) |
| Flexbox rows | Visual direction flips automatically |
| Icons next to text | Icons appear on the **left** of Arabic text |
| Tables | Start from the right |
| Form labels | Right-aligned above inputs |
| Breadcrumbs | Right → left reading order |
| Arrows / chevrons | `>` becomes `<` for "back" |
| Scroll indicators | Right-side scrollbar |

### 9.2 Breakpoints

| Breakpoint | Width | Layout Change |
|-----------|-------|---------------|
| Desktop (default) | ≥ 1200px | Full sidebar + main content |
| Tablet | 992px – 1199px | Sidebar collapses to icons only |
| Mobile | < 992px | Sidebar hides behind hamburger menu |

> **Note:** This is a local store management system, primarily used on desktop workstations or a shared tablet. Mobile is a secondary concern but must not break.

---

## 10 ◈ Motion & Micro-Interactions

### 10.1 Core Animation Principles

```
Fast interactions (< 150ms):  Button hover, input focus
Standard transitions (200ms): State changes, color shifts
Meaningful transitions (300ms): Modal open/close, panel slide
Page transitions (400ms): Full page content fade-in
Data loading (variable): Skeleton loader shimmer
```

### 10.2 Specific Animations

| Element | Animation | Duration |
|---------|-----------|----------|
| Page load | Content fades in, cards stagger 50ms apart | 400ms |
| Dashboard cards | Slide up + fade in on mount | 300ms staggered |
| Chart bars | Grow from base | 800ms easeOutQuart |
| Low-stock badge | Slow pulse ring | 2s loop |
| Sale total update | Number animates (count up/down) | 300ms |
| Delete confirmation | Modal bounces in | 250ms |
| Row added to cart | Briefly highlights `--color-accent-soft` | 500ms |
| Success toast | Slides in from top-right | 300ms, auto-dismiss 3s |
| Error shake | Horizontal oscillation | 400ms |

### 10.3 Loading States

**Skeleton loaders** for all data tables and cards:
```
┌──────────────────────────────────────────────────────┐
│  ████████████████████                  ████████       │  ← shimmer
│  ███████████████████████████   ██████  ████████████   │
│  ████████████  █████████████   ██████  ██████         │
└──────────────────────────────────────────────────────┘
```

- Shimmer color: linear gradient `--color-surface-2 → --color-border → --color-surface-2`
- Animation: 1.5s infinite

---

## 11 ◈ Accessibility Standards

### 11.1 Core Requirements

| Standard | Implementation |
|----------|---------------|
| **Contrast** | All text meets WCAG AA (4.5:1 minimum) |
| **Focus rings** | All interactive elements have visible focus states |
| **Keyboard navigation** | Full tab order, arrow keys for dropdowns |
| **ARIA labels** | All icon-only buttons have `aria-label` in Arabic |
| **Form errors** | Errors linked to inputs via `aria-describedby` |
| **Tables** | Proper `<thead>`, `scope` attributes on headers |
| **Status badges** | Not color-only — include text label |

### 11.2 Focus Visible Style

```css
:focus-visible {
  outline: 3px solid var(--color-accent);
  outline-offset: 2px;
  border-radius: 4px;
}
```

### 11.3 Print Styles

For receipts and reports (`/Sales/Detail`, `/Reports`):
- Remove sidebar, navbar, action buttons
- Print in clean black-and-white
- Receipt width: 80mm (thermal printer compatible)

---

## 12 ◈ Design Tokens Reference

A complete CSS custom properties reference for `site.css`:

```css
:root {
  /* ── Brand Colors ── */
  --color-primary:        #1A3C5E;
  --color-primary-light:  #2C5F8A;
  --color-accent:         #E07B2C;
  --color-accent-soft:    #F5E0CC;

  /* ── Surface Colors ── */
  --color-surface:        #F7F5F2;
  --color-surface-2:      #EDEAE6;
  --color-border:         #D4CFC8;

  /* ── Text Colors ── */
  --color-ink:            #1E1E1E;
  --color-ink-muted:      #6B6560;

  /* ── Status Colors ── */
  --status-good:          #2D7A4F;
  --status-good-bg:       #E8F5EE;
  --status-warn:          #B5860D;
  --status-warn-bg:       #FFF8E1;
  --status-danger:        #B83232;
  --status-danger-bg:     #FFF0F0;
  --status-neutral:       #6B6560;
  --status-neutral-bg:    #F0EEEC;
  --status-info:          #1A3C5E;
  --status-info-bg:       #E8EEF5;

  /* ── Typography ── */
  --font-base:            'Cairo', system-ui, sans-serif;
  --type-display:         2.25rem;
  --type-heading-1:       1.75rem;
  --type-heading-2:       1.375rem;
  --type-heading-3:       1.125rem;
  --type-body:            1rem;
  --type-body-sm:         0.875rem;
  --type-caption:         0.75rem;
  --fw-regular:           400;
  --fw-semibold:          600;
  --fw-bold:              700;

  /* ── Spacing ── */
  --space-1:  4px;
  --space-2:  8px;
  --space-3:  12px;
  --space-4:  16px;
  --space-5:  24px;
  --space-6:  32px;
  --space-7:  48px;
  --space-8:  64px;

  /* ── Radius ── */
  --radius-sm:  6px;
  --radius-md:  8px;
  --radius-lg:  12px;
  --radius-xl:  16px;
  --radius-pill: 9999px;

  /* ── Shadows ── */
  --shadow-sm:    0 1px 3px rgba(0,0,0,0.08);
  --shadow-md:    0 4px 12px rgba(0,0,0,0.10);
  --shadow-lg:    0 8px 24px rgba(0,0,0,0.12);
  --shadow-xl:    0 20px 60px rgba(0,0,0,0.18);

  /* ── Layout ── */
  --sidebar-width:    220px;
  --topnav-height:    60px;
  --content-max-w:    1280px;

  /* ── Transitions ── */
  --transition-fast:     0.15s ease;
  --transition-std:      0.2s ease;
  --transition-slow:     0.3s ease;
}
```

---

```
╔═══════════════════════════════════════════════════════════════════╗
║                                                                   ║
║   AFRAWY STORE  ·  Design Document  ·  v1.0                      ║
║   ─────────────────────────────────────────────────────────────  ║
║   للاستخدام الداخلي فقط  ·  Confidential — Internal Use Only     ║
║                                                                   ║
╚═══════════════════════════════════════════════════════════════════╝
```
