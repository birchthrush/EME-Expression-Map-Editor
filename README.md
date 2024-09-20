# Expression Map Editor (EME)

A standalone Windows application for creating and editing expression maps for use with Cubase. 

![EME Main UI](https://github.com/birchthrush/EME-Expression-Map-Editor/blob/master/EME%20Expression%20Map%20Editor/Resources/Screenshots/main_ui.png)

The basic interface should be familiar to all Cubase users, but contains numerous quality-of-life enhancements over the native editor to speed up the creation of new expression maps, especially complex ones. 

For more information on Cubase Expression Maps, refer to the [Cubase user manual](https://archive.steinberg.help/cubase_pro_artist/v9.5/en/cubase_nuendo/topics/expression_maps/expression_maps_c.html). 

### Requirements

Windows only. Requires .NET 6.0 or newer. 

## Main Features
<details>
  <summary><strong>Create multiple slots or articulations</strong></summary>

  Right-click in either list window to bring up the option to create multiple blank slots or articulation in one go. 
</details>

<details>
  <summary><strong>Multiple and ascending value assignment</strong></summary>

  Many parameters - most notably articulations, colors and midi channels - will be assigned for all selected sound slots. 
  
  Where applicable, hold the *alt* modifier key to assign automatically ascending values. 
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

  If this behavior is not desired, auto-completion can be disabled via a right-click menu option in the Articulation list. 
</details>

<details>
  <summary><strong>Automatic generation of Sound Slot names</strong></summary>

  Accessible via right-click menu. EME will generate names for the selected slots based on their assigned articulations' Description field. If multiple groups are used, the names will be concatenated from left to right. 
</details>

<details>
  <summary><strong>Duplication of slots, copying of output mapping</strong></summary>

  Sound Slots can be duplicated via a right-click option (or the *ctrl+d* key command), retaining all their parameters. This is often useful for managing the combinatorial explosion resulting from using groups, where you often end up with multiple slots that differ only in their group assignments and minor details in output mapping - in such cases, duplicating then modifying slots may be faster than building all from scratch. 

  Also available is the option of copying only the Output Events across multiple slots. When making a multiple selection, the Output section will always reflect the *first* selected slot - this is the data that will be copied across *all* selected sound slots, overwriting any existing Output Events. 
</details>

<details>
  <summary><strong>Copying and automatic incrementation of Output Events</strong></summary>

  The following operations are available as right-click options in the Output Events section. Operations will be carried out (in top-to-bottom order where relevant) on all selected sound slots. Note that the Output Events section always reflects the contents of the *first* selected slot. 
  - Copy output events: with options for automatically incrementing either Data field.
  - Increment nth event: will increment the specified Data field on the nth event (ie: if the first output event in a list is selected, the first event on all slots will be incremented if it exists) on all slots. This in-place modification is sometimes useful when slots have been created via duplication and already have existing output events.

  Typical use case: many instruments have all articulations laid out with ascending keyswitches. Set up and select the appropriate sound slots (with the lowest keyswitched articulation on top), insert a keyswitch OutputEvent on the first event and use the *Copy and increment Data1* command to create ascending keyswitches on all slots. 
</details>

<details>
  <summary><strong>Batch processing of output mapping</strong></summary>

  EME allows rudimentary search-and-replace operations to be performed on sound slots' output events. Select the slots you wish to operate on and select *batch processing* from the right-click menu (or use the *ctrl+h* keyboard shortcut) to bring up the window. 

  ![Batch Processing Window](https://github.com/birchthrush/EME-Expression-Map-Editor/blob/master/EME%20Expression%20Map%20Editor/Resources/Screenshots/batch_processing_window.png)

  The window will display a set of all unique Output Events contained in the selected slots, along with the number of times each event occurs. Select which events you wish to affect and the type of operation: 
  - *Delete* will simply remove all occurrences of the selected events from all selected slots.
  - *Replace* will replace all occurrences of the selected events with the data specified in the lower section of the window.   
</details>

### Keyboard Shortcuts
  | Shortcut | Command                                             |
  | -------- | --------------------------------------------------- |
  | Insert   | Creates new element in list                         |
  | Delete   | Removes selected element(s) from list               |
  | ctrl+n   | Create multiple elements                            |
  | ctrl+d   | Duplicate selected sound slot(s)                    |
  | ctrl+p   | Propagate output events from first selected slot    |
  | ctrl+h   | Batch processing of output events on selected slots |

### A Note on Program Changes
Note that for the sake of consistency with Cubase's conventions, all Program Changes are displayed as 1-128 - and *not* as the more binary accurate 0-127, unlike many other DAW:s and instruments. 

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
While EME has been tested extensively without issues, do note that this program relies on reverse-engineering an undocumented format - as such, perfect reliability cannot be guaranteed. Future versions of Cubase may introduce changes to the format that create compatibility issues. Compatibility has not been verified with other third-party expression map tools/generators. Use at your own risk. 
