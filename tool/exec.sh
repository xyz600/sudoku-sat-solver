#!/bin/bash

BIN_PATH='../src/bin/Debug/netcoreapp2.2/linux-x64'

${BIN_PATH}/sudo-sat-solver init.txt &> problem.cnf
./MiniSat_v1.14_linux problem.cnf result.cnf
python3 decode.py result.cnf 9

