REQUIRE ffl/tst.fs
REQUIRE Cyatq.fs

CREATE MY-NUMBERS 10 CELLS ALLOT
CREATE MY-TREE    40 CELLS ALLOT

T{ ." GET-NUMBER reads the next number from a string " CR
    S" 4807 42 17 " DROP 
    GET-NUMBER 4807 ?S DROP }T

T{ ." GET-NUMBER reads the next negative number from a string " CR
    S" -4807 42 17 " DROP 
    GET-NUMBER -4807 ?S DROP }T


T{ ." GET-NUMBERS reads N numbers from a string and store them " CR
    S" 4807 -42 17 " DROP
    MY-NUMBERS 3 GET-NUMBERS 
    MY-NUMBERS 0 CELLS + @ 4807 ?S
    MY-NUMBERS 1 CELLS + @  -42 ?S
    MY-NUMBERS 2 CELLS + @   17 ?S }T

T{ ." BUILD-LEAVES builds the leaves of sum tree from an array " CR
    S" 42 17 4807 23 -5 1 -100 52 1000 -500 " DROP
    MY-NUMBERS 10 GET-NUMBERS
    MY-TREE MY-NUMBERS 10 BUILD-LEAVES
    MY-TREE 10 FIRST-LEAF-POSITION CELLS + 
    DUP       @ 42 ?S
    CELL+ DUP @ 17 ?S
    CELL+ DUP @ 4807 ?S
    CELL+ DUP @ 23 ?S
    CELL+ DUP @ -5 ?S
    CELL+ DUP @ 1 ?S
    CELL+ DUP @ -100 ?S
    CELL+ DUP @ 52 ?S
    CELL+ DUP @ 1000 ?S
    CELL+ DUP @ -500 ?S
    DROP
}T

T{ ." BUILD-NODES builds the nodes of the sum tree " CR
    MY-TREE 10 BUILD-NODES
    MY-TREE 
    CELL+ DUP @   5337 ?S   \ 4837+500
    CELL+ DUP @   4837 ?S   \ 4889-52
    CELL+ DUP @    500 ?S   \ 500+0
    CELL+ DUP @   4889 ?S   \ 4830+59
    CELL+ DUP @    -52 ?S   \ -4-48
    CELL+ DUP @    500 ?S   \ 500+0
    CELL+ DUP @      0 ?S   \ 0/0
    CELL+ DUP @     59 ?S   \ 42+17
    CELL+ DUP @   4830 ?S   \ 4807+23
    CELL+ DUP @     -4 ?S   \ -5+1
    CELL+ DUP @    -48 ?S   \ -100+52
    CELL+ DUP @    500 ?S   \ 1000-500
    CELL+ DUP @      0 ?S
    CELL+ DUP @      0 ?S
    CELL+ DUP @      0 ?S
    CELL+ DUP @     42 ?S
    CELL+ DUP @     17 ?S
    CELL+ DUP @   4807 ?S
    CELL+ DUP @     23 ?S
    CELL+ DUP @     -5 ?S
    CELL+ DUP @      1 ?S
    CELL+ DUP @   -100 ?S
    CELL+ DUP @     52 ?S
    CELL+ DUP @   1000 ?S
    CELL+ DUP @   -500 ?S
    DROP
}T

T{ ." BUILD-TREE fills the sum tree with sum of the values from an array and the first cell contains the length of initial array " CR
    MY-NUMBERS MY-TREE 10 BUILD-TREE 
    MY-TREE @ 10 ?S 
    MY-TREE 1 CELLS + @ 
    42 17 + 4807 + 23 + -5 + 1 + -100 + 52 + 1000 + -500 + ?S
    MY-TREE 16 CELLS + @ 42 ?S }T

T{ ." QUERY-NODE returns a node value if x and y match a node limit " CR
    0 0 9 9 MY-TREE 1 QUERY-NODE 5337 ?S 
    0 5 9 9 MY-TREE 1 DBG QUERY-NODE 5337 ?S 
    }T
BYE

