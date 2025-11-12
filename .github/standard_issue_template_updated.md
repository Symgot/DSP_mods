---
name: Standard Issue
about: Template für alle Issues - Ausgangspunkt für PR-gesteuerte Bearbeitung
title: "[Typ] Kurztitel des Issues"
labels: []
assignees: []
---

## Titel: [Typ] Kurztitel des Issues

### Problembeschreibung / Feature-Request

Kurze Zusammenfassung des Problems oder gewünschten Features (1–3 Zeilen).

---

## Motivation / Zweck

- Warum soll dieses Issue bearbeitet werden?
- Welchen Nutzen/Mehrwert bringt die Lösung?

---

## Betroffene Komponenten

Liste der betroffenen Module/Dateien/Bereiche im Mod (Pfad/Zeilen falls bekannt).

---

## Quellen / References (ERFORDERLICH)

**WICHTIG:** Jedes Issue MUSS mindestens eine Quelle enthalten. Quellen müssen so genau wie möglich referenziert werden.

### Format für Quellen

| Typ | Beschreibung | URL | Abschnitt |
|-----|--------------|-----|-----------|
| (intern / extern / issue / repo / api-doc / spec) | Kurzbeschreibung | Blob-URL mit commit-SHA oder Tag | Zeilen/Abschnitt |

### Anforderungen an Quellen

- **Typ:** intern (dieses Repo), extern (andere Quelle), issue (anderes Issue), repo (anderes Repo), api-doc (API Dokumentation), spec (Spezifikation)
- **URL:** Blob-URL mit commit-SHA ODER Tag-basiert, niemals Branch-basiert
  - ✓ Richtig: `https://github.com/Symgot/factory-levels-forked/blob/abc123def/docs/design.md#L10-L20`
  - ✓ Richtig: `https://github.com/Symgot/factory-levels-forked/blob/v1.2.3/docs/design.md#L10-L20`
  - ✗ Falsch: `https://github.com/Symgot/factory-levels-forked/blob/main/docs/design.md`
  - ✗ Falsch: `https://github.com/Symgot/factory-levels-forked/blob/develop/docs/design.md`
- **Abschnitt:** Konkrete Zeilen (#L10-L20) oder Paragraph-Referenz

### Beispiele

**Beispiel 1: Interne Dokumentation**
| intern | Designdokument - API Contracts | https://github.com/Symgot/factory-levels-forked/blob/abc123/docs/design.md#L5-L15 | Abschnitt "API Contracts" |

**Beispiel 2: Factorio API**
| api-doc | Factorio API 1.1.0 - LuaEntity.surface | https://lua-api.factorio.com/latest/LuaEntity.html#LuaEntity.surface | Property: surface |

**Beispiel 3: Anderes Repository**
| repo | Factory Balancer Module | https://github.com/Symgot/factory-balancer/blob/def789/src/balancer.lua#L50-L80 | Funktion: BalanceFactory |

---

## Erwartete Lösung

Beschreibe kurz, wie die Lösung aussehen könnte oder welche Schritte zur Umsetzung nötig sind.

**Bei Bugs:** Beschreibe erwartetes vs. aktuelles Verhalten

---

## Reproduktion / Testfälle (bei Bugs)

- Schritte zur Reproduktion des Problems
- Factorio-Version und Mod-Version
- Relevante Savegame-Informationen oder Screenshots

---

## Explizite Dokumentationsanforderungen (optional)

**WICHTIG:** Nur ausfüllen, wenn zusätzliche Dokumentation erforderlich ist (z.B. README-Update, Installation Guide).

Falls ja, beschreibe konkret was dokumentiert werden muss:
- [ ] README.md Update - Abschnitt: _____
- [ ] Installation Guide - Details: _____
- [ ] API Documentation - Details: _____
- [ ] Changelog Entry - Details: _____
- [ ] Andere Dokumentation: _____

**Hinweis:** Ohne explizite Anforderung hier werden KEINE Dokumentationen erstellt. Der Agent erstellt nur Code-Fixes und Tests.

---

## Zugriffsanforderungen (falls Quelle privat)

Falls eine referenzierte Quelle in einem privaten Repo liegt: wer braucht Zugriff und warum? (z. B. Team-Name / GitHub-Handle)

---

## Checkliste (muss VOLLSTÄNDIG abgehakt sein)

- [ ] **Problembeschreibung klar**: Problem/Feature ist verständlich und reproduzierbar
- [ ] **Alle Quellen verlinkt**: "Quellen / References" Sektion vollständig ausgefüllt
- [ ] **Quellen stabil**: Links enthalten commit-SHA oder Versionsangabe (NICHT Branch-Namen)
- [ ] **Abschnitte spezifisch**: Alle Quellen mit Zeilennummern oder Paragraph-Referenzen
- [ ] **Keine sensiblen Quellen**: Private Repos/Dokumente nur mit Berechtigungsangabe
- [ ] **Dokumentationsanforderungen klar**: Falls Dokumentation nötig, explizit angegeben

---

## Workflow-Hinweis

**Wichtig zu verstehen:**

1. **Nach Issue-Erstellung**: Der Agent erstellt einen PR mit der Lösung
2. **Alle Dokumentation im PR**: Die komplette technische Dokumentation, Begründungen und Details sind im PR
3. **Issue wird geschlossen**: Dieses Issue wird nach PR-Erstellung geschlossen
4. **Diskussionen im PR**: Alle weitere Kommunikation, Fragen und Diskussionen erfolgen im PR, nicht hier

**Beispiel-Workflow:**
- Du erstellst dieses Issue mit Quellen
- Der Agent erstellt einen PR mit tabellarischer Auflistung aller Änderungen
- Der Agent kommentiert dieses Issue minimal mit: "Lösung in PR #123 - Alle Details und Dokumentation dort."
- Dieses Issue wird geschlossen → **Alle Details sind jetzt im PR**

---

## Hinweise

- **Keine vagen Quellen:** "Siehe Doku" ist nicht ausreichend. Jede Quelle muss konkret mit URL und Zeilennummern referenziert sein
- **Stabile Links nur:** Verwende immer commit-SHA oder Release-Tags, niemals Branch-Namen (main, develop, etc.)
- **Reproduzierbarkeit:** Diese Links müssen in 6 Monaten noch funktionieren und auf die gleichen Inhalte verweisen
- **Dokumentationsverbot:** Ohne explizite Anforderung im "Dokumentationsanforderungen" Abschnitt werden KEINE README, Guides oder Summaries erstellt

---

## Anweisungen für GitHub Coding Agent

**CRITICAL:** Der GitHub Coding Agent muss folgende Anforderungen beachten:

1. **Dokumentations-Regel beachten**: Keine eigenständige Erstellung von README, Installation Guides, Summaries, etc. außer wenn EXPLIZIT in "Explizite Dokumentationsanforderungen" angegeben
2. **Tabellarischer PR**: Lösung wird als PR mit tabellarischer Auflistung aller Änderungen erstellt
3. **Commit-Quellen**: Jeder Commit enthält Source-URL für Verifikation
4. **Datei-Referenzen**: Alle geänderten Dateien haben Commit-Referenzen
5. **Minimale Issue-Kommentierung**: Nach PR-Erstellung nur minimal kommentieren: "Lösung in PR #<nr> - Alle Details und Dokumentation dort."

---
