# Unity Pixel Text

I've struggled one too many times to get bitmap fonts to work perfectly in Unity--working with TTFs is messy, ensuring that all the various sizes and scales line up perfectly can be a headache and it's the absolute WORST when the pixels don't line up right. This package streamlines things by reading a font from a spritesheet and rendering it using Unity's built-in UI system.

![A pixel text component in the Unity scene view](https://raw.githubusercontent.com/garden-nomes/unity-pixel-text/main/Thumbnail.png)

## Usage

### Installation

Copy the git url for this project and install from the "Package Manager" window in Unity. To use it, first create a "bitmap font" asset, then add a "pixel text" component to your UI.

### Creating a font

1. Arrange all the characters for the given font as a grid and import it as a texture (empty cells will be skipped). Use white text over a transparent background to ensure it renders correctly. (Other, or multiple, foreground colors do work but will blend with the "color" option on the Pixel Text component when rendered). The texture dimensions should be powers of two for compatibility across all platforms (see below).
2. Create a bitmap font: Assets -> Create -> Pixel Text -> Bitmap Font
3. Add the texture from step 1 to the "texture" field
4. The inspector will prompt you about updating some import settings: go ahead and click "Update Import Settings" to have those automatically applied
5. Set "grid width/height" to match the grid from step 1
6. In the "ordering" field enter the characters represented in the bitmap in order, left to right then top to bottom

With all those filled out, the bitmap font asset will automatically scan through the texture cell by cell and determine the actual width of each character, skipping over empty cells. The inspector preview will populate with a rendering of the result.

Note that the "Update Import Settings" button turns off the "non-power of two" import setting, which can modify the texture size and wreaks havoc with the bitmap font system. You should manually size the texture dimensions to a power of two to ensure compatibility on all platforms.

You can adjust some additional settings that control how the font is rendered:

- **Space Width**: width of a space (" ") in pixels
- **Letter Spacing**: number of pixels between each character
- **Line Spacing**: number of pixels between lines

### Creating text

Add a pixel text component: Component -> UI -> Pixel Text

It works as a drop-in replacement for Unity's built-in text component, although missing some features (rich text, for example). Position and size it via rect transform. Long lines will wrap within the rect transform's width. Tweak the "scale" factor (which represents units per pixel) until the pixel size matches your other assets. To make sure everything lines up neatly, the bottom-left corner should align with the pixel grid of your other assets.

## Included fonts

- **m3x6**: A 3 by 6 (on average) pixel font by Daniel Linssen (source: [https://managore.itch.io/m3x6](https://managore.itch.io/m3x6))
