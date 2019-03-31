# -*- coding: utf-8 -*-

import sys


def decode(filepath, size):
    with open(filepath) as fin:
        fin.readline()
        line = fin.readline()
        token = line.split()
        table = [[0 for i in range(size)] for j in range(size)]
        for i in range(size):
            for j in range(size):
                idx = i * size + j
                onehot_list = list(map(int, token[size*idx:size*(idx+1)]))

                for k, val in enumerate(onehot_list, 1):
                    if val > 0:
                        table[i][j] = k
    return table


filepath = sys.argv[1]
size = int(sys.argv[2])

table = decode(filepath, size)
for row in table:
    print(" ".join(map(str, row)))
