
7 CONSTANT NUMBER-SIZE
50000 CONSTANT MAXIMUM-NUMBER
NUMBER-SIZE MAXIMUM-NUMBER * CONSTANT LINE-MAX-LENGTH
CREATE INPUT-LINE LINE-MAX-LENGTH ALLOT
VARIABLE MAX-NUMBER
CREATE NUMBERS MAXIMUM-NUMBER CELLS ALLOT

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

: PRINT-NUMBERS ( m,n -- )
    0 DO
        DUP I CELLS + @ . 
    LOOP DROP ;

: MIDDLE ( l,r -- m )
    OVER - 2/ + ; 

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

\ query-sum t,p,l,r,x,y
\ if x==l && y==r -> t[p]
\ else 
\  m = l + (r-l)/2
\  a <- query-sum t,p*2,l,m,x,min(y,m)
\  b <- query-sum t,p*2+1,m+1,r,max(x,m+1),y
\  -> a+b

\ l,r,ql,qr,m ===> l,m,ql,min(qr,m)
\ OVER OVER MIN \ l,r,ql,qr,m,min(qr,m)

\ a,b,c,d,

: 4DUP  ( a,b,c,d -- a,b,c,d,a,b,c,d )
    2OVER 2OVER ;

: 4DROP ( a,b,c,d -- )
    2DROP 2DROP ;

: SAME-INTERVAL? ( l,ql,r,qr -- f )
    = -ROT = AND ;
: RIGHT-INTERVAL ( l,ql,r,qr,m,m+1 -- m+1,max[ql,m+1] ,r,qr )
    NIP          \ l,ql,r,qr,m+1
    -ROT         \ l,ql,m+1,r,qr
    2>R          \ l,ql,m+1
    ROT DROP     \ ql,m+1
    DUP -ROT MAX \ m+1,max[ql,m+1]
    2R> ;        \ m+1,max[ql,m+1],r,qr 

: QUERY-NODE ( l,ql,r,qr,t,p -- n )
    2>R         \ l,ql,r,qr
    4DUP SAME-INTERVAL?  IF 
        4DROP 
        2R>     \ t,p
        CELLS + @
    ELSE              \ l,ql,r,qr
        MIDDLE DUP 1+ \ l,ql,r,qr,m,m+1
        RIGHT-INTERVAL 
        2R>           \ m+1,max[ql,m+1],r,qr,t,p
        2* 1+ 
        RECURSE
    THEN ;




    

    
