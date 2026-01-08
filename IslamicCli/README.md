# IslamicCli

IslamicCli is a simple, cross-platform command-line application written in plain C# that provides daily Islamic utilities directly in the terminal.

It focuses on prayer times and dhikr, fetched automatically based on your location, without manual configuration.

## Features

- Automatic prayer times based on your location (via IP)
- Display today’s full prayer schedule
- Show the next upcoming prayer
- A list of common Adhkar
- Cross-platform support (Windows, Linux, macOS Intel & Apple Silicon)
- Lightweight, fast, no external dependencies

## Installation

Download the binary for your operating system and place it in a directory included in your PATH.

### macOS / Linux
```bash
chmod +x islamic
sudo mv islamic /usr/local/bin/
```

### Windows (PowerShell)

Place islamic.exe in a folder that is included in your PATH.

### Usage

## Show today’s prayer times:

``` islamic pray ```

## Show the next prayer:

``` islamic pray next ```

## Show a list of common adhkar:

``` islamic dhikr ```

## Show a random dhikr:

``` islamic dhikr --random ```

### Data Sources

Prayer times: AlAdhan API

Dhikr: Local JSON file

### Philosophy

IslamicCli aims to be minimal, respectful, and distraction-free.
No tracking, no ads, no unnecessary complexity.

Built for Muslims who live in the terminal.
