# Pevac
Provides a parser combinator library around the System.Text.Json.Utf8JsonReader class

![Some other name that doesn't matter](https://github.com/BusHero/Pevac/actions/workflows/build.yaml/badge.svg)

## Motivation
Currently it's a pain in the ass to create custom conveters for System.Text.Json. The scope of the project is to simplify this process by providing a Parser Combinator Library around the System.Text.Json.Utf8JsonReader

## Why not use an existing parser combinator library?

**TLDR** It's impossible 

Utf8JsonReader has some restrictions that won't allow to use it in any existing parser combinator library.
1. You cannot specify Utf8JsonReader as a generic parameter
2. All the interaction with the Utf8JsonReader should be through a reference.

Some change here
