REQUIRE ffl/tst.fs
REQUIRE Cyatq.fs

CREATE MY-NUMBERS 10 CELLS ALLOT
CREATE MY-TREE    40 CELLS ALLOT

T{ ." NEXT-NUMBER reads the next number from a string " CR
    S" 4807 42 17 " DROP 
    NEXT-NUMBER 4807 ?S DROP }T

T{ ." NEXT-NUMBER reads the next negative number from a string " CR
    S" -4807 42 17 " DROP 
    NEXT-NUMBER -4807 ?S DROP }T


T{ ." READ-NUMBERS reads N numbers from a string and store them " CR
    S" 4807 -42 17 " DROP
    MY-NUMBERS 3 READ-NUMBERS 
    MY-NUMBERS 0 CELLS + @ 4807 ?S
    MY-NUMBERS 1 CELLS + @  -42 ?S
    MY-NUMBERS 2 CELLS + @   17 ?S }T

T{ ." BUILD-LEAVES builds the leaves of sum tree from an array " CR
    S" 42 17 4807 23 -5 1 -100 52 1000 -500 " DROP
    MY-NUMBERS 10 READ-NUMBERS
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
    0 15 0 9 MY-TREE 1 QUERY-NODE 5337 ?S 
    0 15 0 3 MY-TREE 1 QUERY-NODE 
    42 17 + 4807 + 23 + ?S 
    0 15 6 9 MY-TREE 1 QUERY-NODE 
    -100 52 + 1000 + -500 + ?S 
    0 15 2 5 MY-TREE 1 QUERY-NODE 
    4807 23 + -5 + 1 + ?S 
    }T

T{ ." QUERY-SUM returns a sum of numbers in the array between y and y " CR
    0 9 MY-TREE QUERY-SUM 5337 ?S 
}T

: THAT-TREE
    MY-TREE ;

T{ ." SUM-MAX returns the maximum sum of a series of sums " CR
   ' THAT-TREE IS THE-TREE 
    0 0 SUM-MAX 42 ?S
    0 1 SUM-MAX 59 ?S
    1 2 SUM-MAX 4824 ?S
    4 5 SUM-MAX 1 ?S
    4 7 SUM-MAX 52 ?S
    8 9 SUM-MAX 1000 ?S
    7 9 SUM-MAX 1052 ?S
    0 9 SUM-MAX 5837 ?S
}T

VARIABLE FD-TEMP
CREATE LINE-TEMP 100 ALLOT

T{ ." READ-NUMBER reads a number from a file " CR
    S" TEMP.TXT" W/O CREATE-FILE THROW FD-TEMP !
    S" 4807" FD-TEMP @ WRITE-LINE THROW
    FD-TEMP @ CLOSE-FILE THROW
    S" TEMP.TXT" R/O OPEN-FILE THROW FD-TEMP !
    LINE-TEMP 100 FD-TEMP @ READ-NUMBER
    FD-TEMP @ CLOSE-FILE THROW
    -1 ?S 4807 ?S }T

T{ ." IS-NUMBER? says if a line contains a number " CR
    S"    -4807  " IS-NUMBER? -1 ?S
}T

T{ ." READ-NUMBER returns 0 when no number on the line " CR
    S" TEMP.TXT" W/O CREATE-FILE THROW FD-TEMP !
    S" foo" FD-TEMP @ WRITE-LINE THROW
    FD-TEMP @ CLOSE-FILE THROW
    S" TEMP.TXT" R/O OPEN-FILE THROW FD-TEMP !
    LINE-TEMP 100 FD-TEMP @ READ-NUMBER
    FD-TEMP @ CLOSE-FILE THROW
    0 ?S }T
BYE

