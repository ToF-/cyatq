
7 CONSTANT NUMBER-SIZE
50000 CONSTANT MAXIMUM-NUMBER
NUMBER-SIZE MAXIMUM-NUMBER * CONSTANT LINE-MAX-LENGTH
CREATE INPUT-LINE LINE-MAX-LENGTH ALLOT
VARIABLE MAX-NUMBER
CREATE NUMBERS MAXIMUM-NUMBER CELLS ALLOT
CREATE TREE    MAXIMUM-NUMBER 4 * CELLS ALLOT
VARIABLE RESULT-MAX
0 CONSTANT FAILURE
-1 CONSTANT SUCCESS

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

: READ-NUMBERS ( addr,u1,array,u2,filedesc -- )
    -ROT 2>R >R     \ addr,u1  { u2,array,filedesc }
    OVER SWAP R>    \ addr,addr,u1,filedesc
    READ-LINE THROW IF \ addr,u
        DROP 2R>       \ addr,array,u2
        NEXT-NUMBERS 
    ELSE
        2DROP
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
    
: CREATE-EXAMPLE 1000 0 DO I NUMBERS I CELLS + ! LOOP ;

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

: RESET-RESULT-MAX ( -- )
    INTEGER-MIN RESULT-MAX !    
    ;

DEFER THE-TREE

: RECORD-RESULT-MAX ( x,y -- )
    THE-TREE QUERY-SUM 
    RESULT-MAX @ MAX RESULT-MAX ! ;

' RECORD-RESULT-MAX IS ACTION

: THIS-TREE 
    TREE ;

' THIS-TREE IS THE-TREE

: SUM-MAX ( x,y,t -- n )
    RESET-RESULT-MAX 
    SWAP DO-QUERIES 
    RESULT-MAX @ ;

: READ-NUMBER ( a,n,fd -- n,-1|0 )
    ROT DUP >R -ROT
    READ-LINE THROW 
    IF 
        R@ SWAP IS-NUMBER? IF
            R> NEXT-NUMBER NIP -1
        ELSE
            R> DROP 0
        THEN
    ELSE
        R> 2DROP 0
    THEN ;

    
