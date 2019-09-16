prefix = "\t\t\t"

f = open("ff1.dbg")
# note this only works with --dbgfile output from a recent version of ld65


def grab_all_of_type(typewanted):
    results = dict()
    for line in f:
        typename, kvpairs = line.rstrip('\r\n ').split(None, 1)
        if typename != typewanted:
            continue
        data = dict([pair.split('=', 1) for pair in kvpairs.split(',')])
        results[data['id']] = data
    f.seek(0)
    return results

segments = grab_all_of_type('seg')
symbols  = grab_all_of_type('sym')

for symdata in symbols.values():
    if 'parent' in symdata:
        # local symbol, don't care
        continue

    if 'seg' not in symdata:
        # non-code symbol, we have those from elsewhere
        continue

    # [1:-1] is to get the quotes off of it.
    seg_name = segments[symdata['seg']]['name'][1:-1]

    if seg_name.split("_")[0] != "BANK":
        print("%s// ignored symbol %s in %s" % (prefix, symdata['name'], seg_name))
        continue

    bank_number = seg_name.split("_")[1]

    if bank_number == "FIXED":
        bank_number = "0F"

    sym_name = symdata['name'][1:-1]
    csharp_definition = "public static BA %-35s = new BA(0x%s, %s);" % (sym_name, bank_number, symdata['val'])
    print("%s%s" % (prefix, csharp_definition))

