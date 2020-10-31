# Chunky

A simple program that turns images into chunks.

Under development, wouldn't recommend using it just yet.

## Usage

Current state of the program lacks any user interface, but it'll
have a cli interface that allows using it in scripts.

Chunky accepts two kinds of rules:
* Set width and height
* Set amount of chunks per x and y axis

You can mix and match these, for example
* Divide the image by `10` in x axis and have a height of `100px` each
* Divide the image by `5` in both x and y axis
* Divide the image into equal `256px`x`256px` chunks

## Configuring

WIP