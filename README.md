# Expression Map Editor (EME)

## Features
<details>
  <summary><strong></strong></summary>
</details>

<details>
  <summary><strong>Automatic completion of Articulation descriptors (optional)</strong></summary>

  By convention, the Text field for an articulation is a lower-case abbreviation. EME will automatically generate the full description field by capitalizing each word and expanding selected abbreviated keywords. 

  | Abbreviation | Full Description    |
  | ------------ | ------------------- |
  | s            | Short               |
  | m            | Medium              |
  | l            | Long                |
  | f            | Fast                |
  | sl           | Slow                |
  | tr           | Trills              |
  | stac         | Staccato            |
  | trem         | Tremolo             |
  | det          | Detaché             |
  | marc         | Marcato             |
  | msrd         | Measured            |
  | leg          | Legato              |
  | cresc        | Crescendo           |
  | dim          | Diminuendo          |
  | port         | Portato             |
  | flaut        | Flautando           |
  | cs           | Con Sordino         |
  | ss           | Senza Sordino       |
  | sus          | Sustains            |
  | espr         | Espressivo          |
  | acc          | Accented            |
  | fp           | Fortepiano          |
  | dbl          | Double              |
  | trpl         | Triple              |
  | spic         | Spiccato            |
  | norm         | Normal              |
  | nat          | Natural             |
  | pizz         | Pizzicato           |
  | rep          | Repetitions         |
  | gliss        | Glissando           |
  | sp           | Sul Ponticello      |
  | st           | Sul Tasto           |
  | pp           | Pianissimo          |
  | ff           | Fortissimo          |
  | nv           | Non-Vibrato         |
  | v            | Vibrato             |
  | mv           | Molto Vibrato       |
  | sv           | Strong Vibrato      |
  | pv           | Progressive Vibrato |
  | xf           | Crossfade           |

  If this behavior is not desired, the auto-completion can be disabled via a right-click menu option in the Articulation list. 
</details>

## Known Limitations 
<details>
  <summary><strong>Limited support for musical symbols</strong></summary>

  These are currently shown (and can be assigned) using their underlying numerical code, *not* their proper graphical representation. If you make extensive use of Symbols, it is recommended you use a text attribute as a placeholder and make proper assignment of symbols later within Cubase instead. 
</details>
<details>
  <summary><strong>Limited error checking</strong></summary>

  For example, EME does not explicitly forbid the creation of multiple articulations with identical names - however doing so will result in undefined behavior once imported back into Cubase. Employ common sense.
</details>

## Disclaimer
While EME has been tested extensively without issue, do note that this program relies on reverse-engineering an undocumented format - as such, perfect reliability cannot be guaranteed. Future versions of Cubase may introduce changes to the format that create compatibility issues. Compatibility has not been verified with other third-party expression map tools/generators. Use at your own risk. 
