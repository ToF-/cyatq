REQUIRE ffl/tst.fs
REQUIRE Cyatq.fs


T{ ." NEXT-NUMBER reads the next number from a string " CR
    S" 4807 42 17 " DROP 
    NEXT-NUMBER 4807 ?S DROP }T

T{ ." NEXT-NUMBER reads the next negative number from a string " CR
    S" -4807 42 17 " DROP 
    NEXT-NUMBER -4807 ?S DROP }T


T{ ." NEXT-NUMBERS reads N numbers from a string and store them " CR
    S" 4807 -42 17 " DROP
    NUMBERS 3 NEXT-NUMBERS 
    NUMBERS 0 CELLS + @ 4807 ?S
    NUMBERS 1 CELLS + @  -42 ?S
    NUMBERS 2 CELLS + @   17 ?S }T

T{ ." BUILD-LEAVES builds the leaves of sum tree from an array " CR
    S" 42 17 4807 23 -5 1 -100 52 1000 -500 " DROP
    NUMBERS 10 NEXT-NUMBERS
    TREE NUMBERS 10 BUILD-LEAVES
    TREE 10 FIRST-LEAF-POSITION CELLS + 
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

T{ ." BUILD-NODE-LEVELS builds the nodes of the sum tree " CR
    TREE 10 BUILD-NODE-LEVELS
    TREE 
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
    NUMBERS TREE 10 BUILD-TREE 
    TREE @ 10 ?S 
    TREE 1 CELLS + @ 
    42 17 + 4807 + 23 + -5 + 1 + -100 + 52 + 1000 + -500 + ?S
    TREE 16 CELLS + @ 42 ?S }T

T{ ." QUERY-NODE returns a node value if x and y match a node limit " CR
    0 15 0 9 TREE 1 QUERY-NODE 5337 ?S 
    0 15 0 3 TREE 1 QUERY-NODE 
    42 17 + 4807 + 23 + ?S 
    0 15 6 9 TREE 1 QUERY-NODE 
    -100 52 + 1000 + -500 + ?S 
    0 15 2 5 TREE 1 QUERY-NODE 
    4807 23 + -5 + 1 + ?S 
    }T

T{ ." QUERY-SUM returns a sum of numbers in the array between y and y " CR
    0 9 TREE QUERY-SUM 5337 ?S 
}T

T{ ." SUM-MAX returns the maximum sum of a series of sums " CR
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
: TEMP-FILE-NAME S" TEMP.TXT" ;

: WRITE-TEMP-FILE ( addr,u -- )
    TEMP-FILE-NAME W/O CREATE-FILE THROW FD-TEMP !
    FD-TEMP @ WRITE-LINE THROW
    FD-TEMP @ CLOSE-FILE THROW ;

: OPEN-TEMP-FILE
    TEMP-FILE-NAME R/O OPEN-FILE THROW FD-TEMP ! ;

: CLOSE-TEMP-FILE 
    FD-TEMP @ CLOSE-FILE THROW ;

T{ ." READ-NUMBER reads a number from a file " CR
    S" 4807" WRITE-TEMP-FILE
    OPEN-TEMP-FILE
    LINE-TEMP 100 FD-TEMP @ READ-NUMBER
    CLOSE-TEMP-FILE
    -1 ?S 4807 ?S }T

T{ ." IS-NUMBER? says if a line contains a number " CR
    S"    -4807  " IS-NUMBER? -1 ?S
}T

T{ ." READ-NUMBER returns 0 when no number on the line " CR
    S" foo" WRITE-TEMP-FILE
    OPEN-TEMP-FILE
    LINE-TEMP 100 FD-TEMP @ READ-NUMBER
    CLOSE-TEMP-FILE
    0 ?S }T

T{ ." READ-NUMBERS reads and stores all the numbers on file line " CR
    S" 42 17 23 1 100" WRITE-TEMP-FILE
    OPEN-TEMP-FILE
    LINE-TEMP 100 NUMBERS 5 FD-TEMP @ READ-NUMBERS
    CLOSE-TEMP-FILE
    NUMBERS 0 CELLS + @ 42 ?S 
    NUMBERS 1 CELLS + @ 17 ?S 
    NUMBERS 2 CELLS + @ 23 ?S 
    NUMBERS 3 CELLS + @  1 ?S 
    NUMBERS 4 CELLS + @ 100 ?S 
    }T

T{ ." READ-QUERY reads x and y query parameters on a file " CR
    S" 42 4807 " WRITE-TEMP-FILE
    OPEN-TEMP-FILE
    LINE-TEMP 100 QUERY-ARGS FD-TEMP @ READ-QUERY-ARGS
    CLOSE-TEMP-FILE
    QUERY-ARGS 2@ 42 ?S 4807 ?S 
}T

BYE


