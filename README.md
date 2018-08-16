# PEEdit

This is a PE Header Reader and editor.

## Features

- Reads the entire PE Header of an executable
- Can write changes back to File

## How to use

`PEEdit.exe [/F] [/A] <File> [...]`

- `/F`: Formats the output to be better readable
- `/A`: Uses array based output even if only one file is analyzed
- `File`: File to analyze. Supports Advanced Wildcards.

## Output

The output will be JSON formatted.
Be aware that the JSON output doesn't guarantees that the properties will be in order they appear in the header itself.

Enum based values will be represented as integers, but also as strings under a different property.

Regular output is sent to the standart output stream and error messages to the standard error stream.

### Single file

When supplying only a single file or a mask that only expands to a single file,
the output will only be the JSON representation of the header itself,
unless `/A` is specified which causes the "Multiple Files" behavior to take place.

### Multiple files

If multiple files are specified,
the output will be a JSON dictionary with the fully qualified file name as key and the header as value.

### Unprocessable files

Files that are unprocessable either because they can't be opened
or don't contain a somewhat valid header will be represented as null value.

For single files this means that the output might simply be the string "null".
For multiple files this means that the value of a key can be null.

## Limitations

This application will not modify PE headers at all, only read them.
The writer has been tested and is working, you have to write your own front-end for that for now.

## Integration

To use this in your own application you can either copy the source files or you can set the project to "Class Library"
and delete `Program.cs` to create a fully functional DLL file.
