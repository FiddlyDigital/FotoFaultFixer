# FotoFaultFixer
A .net core class library and WPF UI tool to fix faults in old photos.

This uses techniques from the book "Modern Algorithms for Image Processing" by Vladimir Kovalevsky.
The code here is an modified/optimized version, based on some of the code samples in the book.
It has been heavily refactored for readability/simplicity.

The image processing logic is kept in a seperate library and no longer has dependencies on UI components.
The UI has been changed to WPF from Winforms, and implements the Material Design System for a more modern look.

There's still plenty of work to be done, but the current state is easy to read and has plenty of scope for extension/improvement.
