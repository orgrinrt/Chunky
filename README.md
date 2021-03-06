# Chunky

A simple program that turns images into chunks.

Under development, early stages. Not really usable as of yet.

## Usage

Current state of the program lacks a sophisticated user interface, but base
functionality can be used. Currently available commands:
* `-s` or `--source` `<image_path>` for single-image processing
* `-s` or `--source` `<directory_path>` for a automatic batch operation
* `-h` or `--help` to show currently available commands (always up-to-date, more so than this readme)

Currently the output only saves to `<exe_dir>/<image_name>/<name>-x-y.png`.

Chunky accepts two kinds of rules:
* Set width and height
* Set amount of chunks per x and y axis

You can mix and match these, for example
* Divide the image by `10` in x axis and have a height of `100px` each
* Divide the image by `5` in both x and y axis
* Divide the image into equal `256px`x`256px` chunks

For user's sanity, Chunky can provide (if so configured) a reconstruction based 
on the output files written to disk as well as a variance comparison image that
displays any deviance from the original in bright red.

## Configuring

WIP

## Todo

### Immediate target

* [ ] Serialize/deserialize processing configs and global config (human-readable and easily modifiable)
* [ ] Write a command line interface that is script-friendly
* [ ] As of yet, chunky doesn't handle uneven splits well. It respects the sizes
and produces an accurate output but the edge chunks will have either blank (black)
pixels for the out-of-bounds remainder or it'll glitch in magnificent ways.
    * User-configured handling of uneven remainders
        * [ ] Fill with chosen color
        * [ ] Fill with 100% alpha
        * [ ] Crop the edge chunks (results in irregular sizes for the edge chunks)
    
### Long-term target

* [ ] A simple graphical user interface
* [ ] Cleaning up, refactoring code
* [ ] Performance magic and trying to reduce its footprint