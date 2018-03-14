# CMD ID3 tagger
ID3 command-line tagger for Windows.

## Examples
### Renaming files to tags
Before:
```console
//d/Downloads/
├── 003_STAY BY ME.mp3
├── 01.真夜中のドア／stay with me (1980).mp3
├── 02. Plastic Love (100% City Pop).mp3
├── 2 - 杏里 - WINDY SUMMER.mp3
├── FULL MOON by 八神純子.mp3
├── 大橋純子 - 10 - Dancin'.mp3
└── 大橋純子 - クリスタル・シティー.mp3
```
Run utility:
```cmd
cmdtagger fromtags "D:\\Downloads" "%artist%/%year% - %album%/%track% - %title%"
```
After:
```console
//d/Downloads/
├── 八神純子
│   └── 1983 - FULL MOON
│       └── 9 - FULL MOON.mp3
├── 大橋純子
│   ├── 1977 - クリスタル・シティー
│   │   └── 1 - クリスタル・シティー.mp3
│   └── 1984 - Magical
│       └── 10 - Dancin'.mp3
├── 杏里
│   └── 1983 - Timely!!
│       ├── 2 - WINDY SUMMER.mp3
│       └── 3 - STAY BY ME.mp3
├── 松原みき
│   └── 1980 - Pocket Park
│       └── 1 - 真夜中のドア／stay with me.mp3
└── 竹内まりや
    └── 1984 - VARIETY
        └── 2 - Plastic Love.mp3
```

### Renaming files to tags
Coming soon.

## Downloads
Coming soon.
