
7 CONSTANT NUMBER-SIZE
50000 CONSTANT MAXIMUM-NUMBER
NUMBER-SIZE MAXIMUM-NUMBER * CONSTANT LINE-MAX-LENGTH
CREATE INPUT-LINE LINE-MAX-LENGTH ALLOT
VARIABLE MAX-NUMBER
CREATE NUMBERS MAXIMUM-NUMBER CELLS ALLOT
CREATE TREE    MAXIMUM-NUMBER 4 * CELLS ALLOT
VARIABLE RESULT-MAX

HEX -8000000000000000 DECIMAL CONSTANT INTEGER-MIN

: READ-NUMBERS
    INPUT-LINE DUP LINE-MAX-LENGTH 
    STDIN READ-LINE THROW
    IF EVALUATE ELSE 2DROP THEN ;

: GET-NUMBER ( a -- a',n )
    1 SWAP
    BEGIN
        DUP C@ DIGIT? 0= WHILE 
            DUP C@ 45 = IF 
                SWAP DROP -1 SWAP
            THEN
        1+
    REPEAT
    DROP 0 
    BEGIN 
        OVER C@ DIGIT? WHILE
            SWAP 10 * +
            SWAP 1+ SWAP
    REPEAT 
    ROT * ;

: GET-NUMBERS ( a,m,n -- )
    0 DO                 \ a,m
        SWAP GET-NUMBER  \ m,a',x
        ROT DUP >R !     \ a' -- m <- x
        R> CELL+         \ a',m'
    LOOP 
    2DROP ;

: READ-NUMBER-ARRAY ( n,m -- )
    INPUT-LINE DUP LINE-MAX-LENGTH 
    STDIN READ-LINE THROW
    IF \ n,m,a,n
        DROP SWAP ROT 
        GET-NUMBERS
    ELSE
        2DROP
    THEN ;

: PRINT-NUMBERS ( m,n -- )
    0 DO
        DUP I CELLS + @ . 
    LOOP DROP ;


: FIRST-LEAF-POSITION ( n -- p )
    1 SWAP
    BEGIN
        DUP 0> WHILE
            2/ SWAP 2* SWAP
    REPEAT DROP ;

: BUILD-NODE ( t,p -- )
    OVER OVER \ t,p,t,p
    2* CELLS + \ t,p,tp*2
    DUP CELL+  \ t,p,tp*2,tp*2+1
    @ SWAP @ + \ t,p,v
    -ROT CELLS + ! ;  \ t[p]<-v
    
    
: BUILD-LEAVES ( t,a,n -- )
    DUP >R FIRST-LEAF-POSITION \ t,a,p
    CELLS ROT + SWAP           \ tp,a
    R> 0 DO 
        OVER OVER           \ tp,a,tp,a
        @ SWAP !            \ tp,a -- tp <- [a] 
        CELL+ SWAP          \ a+1,tp
        CELL+ SWAP          \ tp+1,a+1
    LOOP 2DROP ;

: BUILD-NODE-LEVEL ( t,p -- )
    DUP 2/ DO
        DUP I 2* CELLS +  \ t,t2i
        DUP CELL+         \ t,t2i,t2i+1
        @ SWAP @ +        \ t,v
        OVER I CELLS + !  \ t -- t[i] <- v
    LOOP DROP ;

: BUILD-NODES ( t n -- )
    FIRST-LEAF-POSITION \ t,p
    OVER OVER
    BEGIN
        2DUP BUILD-NODE-LEVEL
        2/
    DUP 1 = UNTIL 
    2DROP 2DROP ;

: BUILD-TREE ( a,t,n -- )
    OVER !    \ a,t 
    DUP ROT OVER @ \ t,t,a,n
    BUILD-LEAVES   \ t
    DUP @          \ t,n
    BUILD-NODES ;


: MIDDLE ( l,r -- m )
    OVER - 2/ + ; 

: INTERVALS ( l,r -- l,m,m+1,r )
    OVER OVER MIDDLE  \ l,r,m
    DUP 1+ ROT ;      \ l,m,m+1,r

: QUERY-NODE ( l,r,x,y,t,p -- n )
    2>R
    2DUP > IF
        2DROP 2DROP
        2R> 2DROP
        0
        EXIT
    THEN
    2OVER 2OVER D= IF
        2DROP 2DROP
        2R>
        CELLS + @
    ELSE
        2SWAP INTERVALS  \ x,y,l,m,m+1,r
        2>R DUP >R       \ x,y,l,m
        2OVER            \ x,y,l,m,x,y
        R> MIN           \ x,y,l,m,x,min m y'
        2ROT             \ l,m,x,y',x,y
        2R>              \ l,m,x,y',x,y,m+1,r
        OVER >R          
        2SWAP            \ l,m,x,y',m+1,r,x,y
        SWAP R> MAX SWAP \ l,m,x,y',m+1,r,x'1,y
        2R@ 2* 1+ RECURSE
        2R> ROT >R 2* RECURSE 
        R> +  
    THEN ;

: QUERY-SUM ( x,y,t -- s )
    DUP @                    \ x,y,t,n
    FIRST-LEAF-POSITION 1-   \ x,y,t,r 
    0 SWAP                   \ x,y,t,l,r
    ROT >R 2SWAP R> 1        \ l,r,x,y,t,1
    QUERY-NODE ;
    
: CREATE-EXAMPLE 1000 0 DO I NUMBERS I CELLS + ! LOOP ;

DEFER ACTION

: DO-QUERY ( y,x -- action on x,x then x,x+1 .. x,y )
    DUP -ROT
    DO
        DUP I ACTION
    LOOP DROP ;

: DO-QUERIES ( y,x -- action on series x,x, x,x+1, x,y, x+1,x+1, x+1,x+2, ... )
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

: PROCESS
    READ-NUMBERS
    DUP MAX-NUMBER !
    NUMBERS READ-NUMBER-ARRAY 
    NUMBERS TREE MAX-NUMBER @ BUILD-TREE
    READ-NUMBERS
    0 DO
        READ-NUMBERS
        1- SWAP 1- SWAP TREE
        SUM-MAX
        . CR
    LOOP ;
    
CR DBG PROCESS BYE
