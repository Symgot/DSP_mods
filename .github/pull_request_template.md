---
name: Pull Request
about: Tabellarischer PR für Agent-gesteuerte Issue-Bearbeitung
title: "[PR] Kurztitel der Änderung"
---

# Bezug zum Issue

Fixes #<issue-number>

---

## Änderungen (Tabellarisch)

**WICHTIG:** Diese Tabelle listet ALLE Änderungen detailliert auf. Sie ist der primäre Nachweis für die Bearbeitung.

| Datei | Änderungstyp | Änderung | Commit-SHA | Quelle |
|-------|--------------|----------|-----------|--------|
| `<Dateipath>` | Added/Modified/Deleted | Zeile X-Y: <Beschreibung der Änderung> | `<sha7>` | [Quellenname](URL) |
| | | | | |

**Ausfüllhinweise:**
- **Datei**: Vollständiger Dateipfad (relativ zum Repo-Root)
- **Änderungstyp**: Added (neue Datei), Modified (bestehende Datei geändert), Deleted (gelöschte Datei)
- **Änderung**: Maximal detailliert - Zeilennummern, Funktionsnamen, spezifische Codeblöcke
  - Beispiel: `Zeile 5-12: Funktion validate_version() hinzugefügt mit Regex-Pattern`
  - Beispiel: `Zeile 42: Parameter type_hint in FunctionX von Optional[str] zu str geändert`
- **Commit-SHA**: Kurz-SHA (7 Zeichen) des Commits, der diese Änderung enthält
- **Quelle**: Markdown-Link zur Quellen-URL mit Commit-SHA oder Tag

---

## Commits mit Quellen

Alle Commits müssen nach diesem Format erstellt werden:

```
<Typ>: <Kurzbeschreibung> (fixes #<issue>) - Source: <URL>

<Optional: Detaillierte Beschreibung im Body>
- Zeile X-Y: <Was wurde geändert>
- Zeile A-B: <Was wurde geändert>
```

**Beispiel:**
```
Fix: Validate version tags correctly (fixes #123) - Source: https://github.com/org/repo/blob/abc123/docs/api.md#L5-L12

- src/validator.py Zeile 5-12: VERSION_REGEX Pattern hinzugefügt
- tests/test_validator.py Zeile 15-18: Zwei Testfälle für v-Tags hinzugefügt
```

---

## Datei-Referenzierung (mit Commits)

In jeder geänderten/erstellten Datei **MUSS** zu jeder Änderung der zugehörige Commit referenziert werden:

```python
# Modified in commit abc123d: Validate version tags correctly
# Source: https://github.com/org/repo/blob/main/docs/api.md#L5-L12
def validate_version(version_str: str) -> bool:
    VERSION_REGEX = re.compile(r"v?\d+\.\d+(\.\d+)?")  # Added in abc123d
    ...
```

---

## Quellen / References (ERFORDERLICH)

Alle referenzierten Quellen MÜSSEN verifizierbar sein:

| Typ | Beschreibung | URL | Abschnitt |
|-----|--------------|-----|-----------|
| intern / extern / issue / repo | Kurzbeschreibung | Blob-URL mit commit-SHA | Zeilen/Abschnitt |

**Format für Quellen:**
- **Typ:** intern (dieses Repo), extern (andere Quelle), issue (anderes Issue), repo (anderes Repo)
- **URL:** Blob-URL mit commit-SHA ODER Tag, nicht Branch
  - ✓ Richtig: `https://github.com/org/repo/blob/abc123def/path/file.md#L10-L20`
  - ✗ Falsch: `https://github.com/org/repo/blob/main/path/file.md`
- **Abschnitt:** Konkrete Zeilen oder Paragraph-Referenz

---

## Erklärung zur Verifikation (ERFORDERLICH)

Beschreibe, wie die Änderungen gegen die referenzierten Quellen verifiziert werden können:

- "Vergleiche die Validierungslogik in src/validator.py (Zeile 5-12) mit dem Specification Document (Zeilen 10-20)"
- "Prüfe, dass alle Test-Cases in tests/test_validator.py (Zeilen 15-18) die Cases aus dem API Spec (Zeilen 5-12) abdecken"

---

## Tests / CI

- Welche Tests wurden hinzugefügt/aktualisiert?
- Link zu Testfällen (Pfad oder blob-URL mit commit-SHA):
  - Beispiel: `tests/test_validator.py` (commit abc123d)
  - Beispiel: `https://github.com/org/repo/blob/abc123d/tests/test_validator.py#L15-L18`

---

## Zugriffsanforderungen (falls Quelle privat)

Falls eine referenzierte Quelle in einem privaten Repo oder Dokument liegt: wer braucht Zugriff und warum?

---

## Checkliste (muss VOLLSTÄNDIG abgehakt sein)

- [ ] **Änderungen-Tabelle ausgefüllt**: Alle Dateien mit Zeilennummern, Commit-SHAs und Quellen
- [ ] **Commits mit Quellen**: Jeder Commit enthält Source-URL
- [ ] **Datei-Referenzierung**: Alle geänderten Dateien haben Commit-Referenzen als Kommentare
- [ ] **Quellen verifizierbar**: Alle URLs enthalten commit-SHA oder Tag (NICHT main/master)
- [ ] **Abschnitte spezifisch**: Alle Quellen mit Zeilen- oder Paragraph-Nummern
- [ ] **Verifikation dokumentiert**: Klare Schritte zur Überprüfung der Änderungen
- [ ] **Tests aktualisiert/verlinkt**: Alle Tests mit blob-URL und commit-SHA
- [ ] **Keine sensiblen Inhalte**: Private Quellen nur mit Berechtigungsangabe

---

## Hinweis zur Dokumentation

**WICHTIG:** Alle Dokumentation und Diskussion erfolgt AUSSCHLIEßLICH in diesem PR, nicht im zugehörigen Issue.

- Das Issue dient nur als Ausgangspunkt und wird nach PR-Erstellung geschlossen
- Technische Details, Begründungen und Quellendiskussionen gehören HIER in den PR
- Keine eigenständige Erstellung von README, Installation Guides, Summaries etc., außer wenn EXPLIZIT im Issue gefordert

---
