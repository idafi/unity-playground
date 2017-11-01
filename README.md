# unity-playground
Assorted C# tools built for use with the Unity3D game engine.

---

## TileMap
Storage-efficient tile mapping system.

The TileMapData object organizes TileSets, Sprite references, and Collider data into a series of packed index IDs. The TileMap then uses this data to generate Tiles at runtime, which can then be drawn via the TileMapRenderer, and used to generate a collision mesh through the TileMapCollider. Tiles can be directly painted into the scene editor through the TileMapEditor.

The TileMapRenderer dynamically builds a series of meshes, and draws them directly to the specified Cameras. Tiles are organized into arbitrarily sized "chunks", each of which is represented by a single Mesh. The Mesh is then then selectively created, rendered, and destroyed based on its constituent Tiles' visibility.

The TileMapCollider constructs a map-wide collision mesh at runtime, based on per-tile collision information. Both full-square collision meshes and arbitrary polygons are supported, combined using Unity's CompositeCollider system. To prevent overflowing the hierarchy, collider objects are also organized into arbitrarily-sized "chunks" with their own container objects.

The TileMapEditor contains a modular "toolbox," including tools like a Painter and Eraser, that can be used to directly "paint" tiles into the scene. These operate directly on the unity-serialized TileMapData object, and consequently retain automatic serialization and full undo/redo support. Most editor scripts make use of "CED", a customized but largely transparent wrapper around Unity's inspector API, not provided here.

"Then why upload it" now that Unity has its own native tilemapping system, the fully-custom solution is rendered basically irrelevant. The work done here will likely find its way into my 2D game engine, later down the line.

## WordScript
Parses screenplay-like text into in-game dialouge exchanges.

A Custcene is authored using a Script and a series of IActors. An IActor has a player-visible Name and a Portrait collection, providing visualizations of an arbitrary set of emotional states. Cutscenes then combine these elements into Wordsboxes, which present the parsed text to the player.

A Script's format is an extremely simple screenplay-like block of text -- see the *sample script.txt* for an example. Lines ended by a colon ':' denote the name of an actor; line breaks ask the user for input to continue dialogue; and a |keyword| surrounded by vertical bars triggers a Portrait change. All other text is preserved and presented as-is.

The Wordsbox uses the Name and Portrait data provided by an IActor to present an isolated segment of the Script (an array of Word objects). Text is printed one character at a time, to represent the sentence being gradually formed as it is "spoken." Special punctuation characters can be defined to alter the speed at which text will print -- to mimic a pause at the end of a sentence, for example -- but permit exceptions, such as "Mr." or "Mrs."

The end-user simply has to provide a set of IActors, author a Script-compatible text file, and set up any ^punctuation data. The Cutscene can then fully generate in-game dialogue between multiple actors.
