# Required marching square tiles for n types with rotational symmetry


def get_tiles(types):
    tiles = []

    def add_not_duplicate(tile):
        for m in range(0, 4):
            if((tile[m:] + tile[:m]) in tiles):
                return
        tiles.append(tile)

    for i in range(0, types):
        for j in range(0, types):
            for k in range(0, types):
                for l in range(0, types):
                    add_not_duplicate([i, j, k, l])
    
    return tiles

types = 3
tiles = get_tiles(types)
print(types**4)
print(len(tiles))
for tile in tiles:
    print(tile)

for types in range(1, 10):
    print(types**4/len(get_tiles(types)))