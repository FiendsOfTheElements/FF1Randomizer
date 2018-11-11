import re

prefix = "\t\t\t"
SYMBOL_DEF_LINE_RE = re.compile(r'^\s*'   # optional whitespace
                                r'(?:'   # then maybe
                                    r'([a-z0-9A-Z_]+)'  # symbol id, group 0
                                    r'\s*=\s*'       # = 
                                    r'([^;]+?)'       # everything up to the first ; is group 1
                                r'\s*)?'  # optional whitespace
                                r'(?:;;?(.+))?'      # maybe comments, comment text is group 2
                                r'\s*$'  # optional whitespace
                                )
HEX_NUMBERS_RE = re.compile(r'\$([A-Fa-f0-9]+)')
BIN_NUMBERS_RE = re.compile(r'%([01]+)')

f = open("Constants.inc")

for line in f:
    line = line.rstrip('\n\r ')
    # print(">>>%s<<<" % line)
    if line == "":
        print()
        continue
    if line.lstrip().startswith(';'):
        # comment line
        # print("\t\t\t// " + )
        pass
    matches = SYMBOL_DEF_LINE_RE.fullmatch(line)
    if matches is not None:
        lvalue, rvalue, comments = matches.groups()

        if lvalue is None and rvalue is None:
            if comments == ';':
                print("%s//" % prefix)
            elif re.fullmatch(';+', comments):
                # it's all semicolons!
                print("%s// ----------------" % prefix)
            else:
                print("%s//%s" % (prefix, comments))
            continue

        rvalue = HEX_NUMBERS_RE.sub(r'0x\1', rvalue)
        rvalue = BIN_NUMBERS_RE.sub(r'0b\1', rvalue)

        definition = "public static int %-20s = %s;" % (lvalue, rvalue)
        maybe_comment = "// " + comments if comments is not None else ""
        print("%s%-54s %s" % (prefix, definition, maybe_comment))

