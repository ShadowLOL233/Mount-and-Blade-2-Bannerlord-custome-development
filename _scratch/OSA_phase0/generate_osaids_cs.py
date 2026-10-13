"""
Generate OpenSourceArmouryRBMBalance/src/OSAIds.cs from strict reclassifier Tier A.
Reads: strict_pom.tierA.json / strict_pbs.tierA.json / strict_cfm.tierA.json
Writes: ../OpenSourceArmouryRBMBalance/src/OSAIds.cs
"""
import json, os, sys
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))
TARGET = os.path.normpath(os.path.join(HERE, '..', '..',
    'OpenSourceArmouryRBMBalance', 'src', 'OSAIds.cs'))

def load(name):
    with open(os.path.join(HERE, name), 'r', encoding='utf-8') as f:
        return sorted(set(json.load(f)))

pom = load('strict_pom.tierA.json')
pbs = load('strict_pbs.tierA.json')
cfm = load('strict_cfm.tierA.json')

def fmt_array(ids, indent=12):
    lines = []
    for i in ids:
        # Validate: no quotes, no commas, pure ASCII-ish identifiers.
        if '"' in i or ',' in i:
            raise ValueError(f'Suspect ID: {i!r}')
        lines.append(' ' * indent + f'"{i}",')
    return '\n'.join(lines)

content = f'''namespace OSABalance.RBMBridge
{{
    // Phase 0.B production list — generated 2026-10-13 from strict reclassifier Tier A
    // (see _scratch/OSA_phase0/reclassify_strict.py + strict_report.md).
    //
    // Classification rules (vanilla semantics derived from RBMCombat.decompiled.cs):
    //   POM = BodyArmor Plate with 'scale'/'lamellar'/'over mail'/'reticulated plate'/
    //         'segmentata' cue in id or name
    //   PBS = BodyArmor Plate with 'plated_leather'/'fortified'/'heavy_armor'/'raider'/
    //         'warlord'/'breastplate'/'full_plate'/'coat_of_plates' cue (and NO POM cue)
    //   CFM = HeadArmor with closed-face cue (closed/goggled/faceguard/bascinet/visor)
    //         AND mail-equivalent cue (mail/aventail/coif/lamellar/vendel) AND NOT 'Open'
    //
    // Tier B items (CFM 62 + PBS 1 edge cases) are NOT in this list — pending manual
    // review in follow-up session. See strict_cfm.tierB.json / strict_pbs.tierB.json.
    //
    // Counts (vanilla + Tier A combined):
    //   POM: 20 + {len(pom)} = {20 + len(pom)}
    //   PBS:  9 + {len(pbs)} = {9 + len(pbs)}
    //   CFM: 16 + {len(cfm)} = {16 + len(cfm)}
    internal static class OSAIds
    {{
        public static readonly string[] PlateOverMail = new[]
        {{
{fmt_array(pom)}
        }};

        public static readonly string[] PlateBelowShoulder = new[]
        {{
{fmt_array(pbs)}
        }};

        public static readonly string[] ClosedFaceMail = new[]
        {{
{fmt_array(cfm)}
        }};
    }}
}}
'''

with open(TARGET, 'w', encoding='utf-8', newline='\n') as f:
    f.write(content)
print(f'Wrote {TARGET}')
print(f'  POM: {len(pom)}  PBS: {len(pbs)}  CFM: {len(cfm)}  (Tier A OSA items)')
