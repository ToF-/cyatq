
7 CONSTANT NUMBER-SIZE
50000 CONSTANT MAXIMUM-NUMBER
NUMBER-SIZE MAXIMUM-NUMBER * CONSTANT LINE-MAX-LENGTH
CREATE INPUT-LINE LINE-MAX-LENGTH ALLOT
VARIABLE MAX-NUMBER
VARIABLE MAX-QUERIES
CREATE NUMBERS MAXIMUM-NUMBER CELLS ALLOT
CREATE TREE    MAXIMUM-NUMBER 4 * CELLS ALLOT
VARIABLE RESULT-MAX
0 CONSTANT FAILURE
-1 CONSTANT SUCCESS
2VARIABLE QUERY-ARGS

: 3DUP ( a,b,c -- a,b,c,a,b,c )
    DUP 2OVER ROT ; 
    
HEX -8000000000000000 DECIMAL CONSTANT INTEGER-MIN

: IS-DIGIT? ( char -- flag )
    DUP  [CHAR] 0 >= 
    SWAP [CHAR] 9 <= AND ;

: SKIP-NON-DIGIT ( addr -- addr )
    BEGIN 
        DUP C@          
        DUP IS-DIGIT?   
        SWAP [CHAR] - = OR
    0= WHILE 
        1+ 
    REPEAT ;

: IS-NUMBER? ( addr,u -- flag )
    OVER + >R        \ addr -- limit in return stack
    SKIP-NON-DIGIT 
    DUP R@ < IF      \ not out of limit?
        DUP C@ [CHAR] - = IF 1+ THEN
        DUP R> < IF  \ not out of limit?
             C@ IS-DIGIT?  \ at least one digit ?
        ELSE
            FAILURE
        THEN
    ELSE
        R> 2DROP 
        FAILURE
    THEN ;

: ACCUMULATE-NUMBER ( u,char -- u )
    [CHAR] 0 - SWAP
    10 * + ;

: NEXT-UNUMBER ( addr -- addr',u )
    0                 \ addr,acc
    BEGIN 
        OVER C@       \ addr,acc,char
        DUP IS-DIGIT? \ addr,acc,char,flag
    WHILE 
        ACCUMULATE-NUMBER  \ addr,acc
        SWAP 1+ SWAP       \ addr',acc
    REPEAT \ addr,acc,char
    DROP ; 

: NEXT-NUMBER ( addr -- addr',n )
    SKIP-NON-DIGIT   \ addr
    DUP C@           \ addr,char
    [CHAR] - = IF 1+ -1 ELSE 1 
               THEN  \ addr,sign
    SWAP             \ sign,addr
    NEXT-UNUMBER     \ sign,addr,u
    ROT * ;          \ addr,n
    
: NEXT-NUMBERS ( addr,array,u -- )
    0 DO                 \ addr,array
        SWAP NEXT-NUMBER \ array,addr',n
        ROT SWAP OVER    \ addr',array,n,array
        ! CELL+          \ addr',array++
    LOOP 
    2DROP ;

\ read a line, filling it with 0 first
: READ-NEW-LINE ( addr,u,filedesc -- u,flag )
    >R 2DUP 0 FILL R>
    READ-LINE THROW ;
\ read a number on a file, return number and a true flag, or false
: READ-NUMBER ( addr,u,filedesc -- n,#true | #false )
    ROT DUP 2SWAP       \ addr,addr,u,filedesc
    READ-NEW-LINE IF    \ addr,u
        OVER SWAP       \ addr,addr,u
        IS-NUMBER? IF   \ addr
            NEXT-NUMBER \ addr',n
            NIP SUCCESS \ n,-1
        ELSE
            DROP 0
        THEN
    ELSE
        2DROP 0
    THEN ;

: READ-NUMBERS ( addr,u1,array,u2,filedesc -- )
    >R 2SWAP OVER SWAP R>    \ array,u2,addr,addr,u1,filedesc
    READ-LINE THROW IF       \ array,u2,addr,u3
        DROP -ROT            \ addr,array,u2
        NEXT-NUMBERS
    ELSE
        DROP 2DROP 
    THEN ;

: READ-QUERY-ARGS ( addr,u,2var,filedesc -- )
    >R -ROT OVER SWAP R> \ 2var,addr,addr,u,filedesc
    READ-LINE THROW IF   \ 2var,addr,u2
        DROP SWAP 2      \ addr,2var,2
        NEXT-NUMBERS
    ELSE
        DROP 2DROP
    THEN ;

: PRINT-NUMBERS ( array,u -- )
    0 DO
        DUP I CELLS + @ . 
    LOOP DROP ;

\ position of the first leaves in a tree
: FIRST-LEAF-POSITION ( size -- pos )
    1 SWAP
    BEGIN
        DUP 0> WHILE
            2/ SWAP 2* SWAP
    REPEAT DROP ;


\ build all the leaves in the tree
: BUILD-LEAVES ( tree,array,u -- )
    DUP >R FIRST-LEAF-POSITION \ tree,array,pos
    CELLS ROT + SWAP           \ tree+pos,array
    R> 0 DO 
        OVER OVER           \ tree,array,tree,array
        @ SWAP !            \ tree,array  -- store in tree
        CELL+ SWAP          \ array',tree
        CELL+ SWAP          \ tree',array'
    LOOP 2DROP ;

\ build all the nodes at a level e.g 4..7
: BUILD-NODE-LEVEL ( tree,pos -- )
    DUP 2/ DO             \ from pos/2 to pos ..
        DUP I 2* CELLS +  \ tree,tree+i*2
        DUP CELL+         \ tree,tree+i*2,tree+i*2+1
        @ SWAP @ +        \ tree,right+left
        OVER I CELLS + !  \ tree -- tree+i <- right+left 
    LOOP DROP ;

\ build all the nodes level
: BUILD-NODE-LEVELS ( tree,size -- )
    FIRST-LEAF-POSITION      \ tree,pos
    BEGIN
        2DUP BUILD-NODE-LEVEL \ tree,pos 
        2/                    \ tree,pos/2
    DUP 1 = UNTIL 
    2DROP ;

\ build a sum tree from an array
: BUILD-TREE ( array,tree,size -- )
    OVER !    \ store tree size in tree[0]
    DUP ROT OVER @ \ tree,tree,array,size
    BUILD-LEAVES   \ tree
    DUP @          \ tree,size
    BUILD-NODE-LEVELS ;


: MIDDLE ( left,right -- middle )
    OVER - 2/ + ; 

: INTERVALS ( left,right -- left,middle,middle+1,right )
    OVER OVER MIDDLE  \ left,right,middle
    DUP 1+ ROT ;      

: SAME-INTERVALS? ( left,right,x,y -- flag )
    D= ;

: QUERY-NODE ( left,right,x,y,tree,pos -- sum )
    2>R               \ { pos,tree }
    2DUP <= IF
        2OVER 2OVER 
        SAME-INTERVALS? IF
            2DROP 2DROP 2R>
            CELLS + @
        ELSE                  \ left,right,x,y
            2SWAP INTERVALS   \ x,y,left,middle,middle+1,right
            2>R DUP >R        \ x,y,left,middle                { right,middle+1,middle }
            2OVER             \ x,y,left,middle,x,y
            R>                \ x,y,left,middle,x,y,middle     { right,middle+1 }
            MIN               \ x,y,left,middle,x,min y middle 
            2ROT              \ left,middle,x,middle y min,x,y
            2R>               \ left,middle,x,middle y min,x,y,middle+1,right 
            OVER >R           \ left,middle,x,middle y min,x,y,middle+1,right { middle+1 }
            2SWAP             \ left,middle,y,middle y min,middle+1,right,x,y 
            SWAP R>           \ left,middle,y,middle y min,middle+1,right,y,x,middle+1 
            MAX SWAP          \ left,middle,y,middle y min,middle+1,right,middle+1 x max,y 
            2R@               \ left,middle,y,middle y min,middle+1,right,middle+1 x max,y,tree,pos
            2* 1+             \ left,middle,y,middle y min,middle+1,right,middle+1 x max,y,tree,pos*2+1 
            RECURSE           \ left,middle,y,middle y min,sumr
            2R>               \ left,middle,y,middle y min,sumr,tree,pos
            ROT >R            \ left,middle,y,middle y min,tree,pos   { sumr }
            2*                \ left,middle,y,middle y min,tree,pos*2
            RECURSE           \ suml
            R> +              \ suml+sumr
        THEN
    ELSE   \ x > y : out of limit
        2DROP 2DROP 2R> 2DROP
        0
    THEN ;

: QUERY-SUM ( x,y,tree -- sum )
    DUP @                    \ x,y,tree,size
    FIRST-LEAF-POSITION 1-   \ x,y,tree,pos-1 
    0 SWAP                   \ x,y,tree,0,pos-1
    ROT >R                   \ x,y,0,pos-1 { tree }
    2SWAP R> 1               \ 0,pos-1,x,y,tree,1 
    QUERY-NODE ;
    
DEFER ACTION

\ execute ACTION on paramaters x,x then x,x+1 til x,y
: DO-QUERY ( y,x -- )
    DUP -ROT            \ x,y,x
    DO                  \ from x to y
        DUP I           \ x,i
        ACTION    
    LOOP DROP ;

\ execute ACTION on series x,y, x,x+1 .. x,y then x+1,x+1, x+1,x+2, .. x+1,y .. y,y
: DO-QUERIES ( y,x -- )
    SWAP 1+ DUP ROT DO
        DUP I DO-QUERY
    LOOP DROP ;

: PRINT2NUMBERS ( x,y -- )
    SWAP . . CR ;

' PRINT2NUMBERS IS ACTION

: PRINT-QUERY-SUM ( x,y -- )
    TREE QUERY-SUM . CR ;


\ launch a query on the given tree
: RECORD-RESULT-MAX ( x,y -- )
    TREE QUERY-SUM 
    RESULT-MAX @ MAX RESULT-MAX ! ;

' RECORD-RESULT-MAX IS ACTION

\ computes the maximum sum of series i,j | x <= i <= j <= y
: SUM-MAX ( x,y -- n )
    INTEGER-MIN RESULT-MAX !    
    SWAP DO-QUERIES 
    RESULT-MAX @ ;


\ reads numbers, then queries, then produce results
: PROCESS ( addr,u,filedesc -- )
    3DUP                 \ addr,u,filedesc,addr,u,filedesc
    READ-NUMBER IF       \ addr,u,filedesc
        MAX-NUMBER !
        3DUP             \ addr,u,filedesc,addr,u,filedesc
        NUMBERS MAX-NUMBER @ ROT \ adr,u,filedesc,addr,u,array,u2,filedesc
        READ-NUMBERS             \ addr,u,filedesc
        NUMBERS TREE MAX-NUMBER @ \ addr,u,filedesc,array,tree,u2
        BUILD-TREE
        3DUP READ-NUMBER IF
            MAX-QUERIES !
            MAX-QUERIES @ 0 DO
                3DUP QUERY-ARGS SWAP READ-QUERY-ARGS
                QUERY-ARGS 2@ 1- SWAP 1- 
                SUM-MAX . CR
            LOOP
            DROP 2DROP
        ELSE
            ABORT" MISSING NUMBER OF QUERIES"
        THEN
    ELSE
        ABORT" MISSING: SIZE OF ARRAY"
    THEN
;

    
