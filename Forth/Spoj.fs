
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
    OVER + >R        
    SKIP-NON-DIGIT 
    DUP R@ < IF      
        DUP C@ [CHAR] - = IF 1+ THEN
        DUP R> < IF  
             C@ IS-DIGIT?  
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
    0                 
    BEGIN 
        OVER C@       
        DUP IS-DIGIT? 
    WHILE 
        ACCUMULATE-NUMBER  
        SWAP 1+ SWAP       
    REPEAT 
    DROP ; 

: NEXT-NUMBER ( addr -- addr',n )
    SKIP-NON-DIGIT   
    DUP C@           
    [CHAR] - = IF 1+ -1 ELSE 1 
               THEN  
    SWAP             
    NEXT-UNUMBER     
    ROT * ;          
    
: NEXT-NUMBERS ( addr,array,u -- )
    0 DO                 
        SWAP NEXT-NUMBER 
        ROT SWAP OVER    
        ! CELL+          
    LOOP 
    2DROP ;


: READ-NEW-LINE ( addr,u,fd -- u,flag )
    >R 2DUP 0 FILL R>
    READ-LINE THROW ;

: READ-NUMBER ( addr,u,fd -- n,#true | #false )
    ROT DUP 2SWAP       
    READ-NEW-LINE IF    
        OVER SWAP       
        IS-NUMBER? IF   
            NEXT-NUMBER 
            NIP SUCCESS 
        ELSE
            DROP 0
        THEN
    ELSE
        2DROP 0
    THEN ;

: READ-NUMBERS ( addr,u1,array,u2,fd -- )
    >R 2SWAP OVER SWAP R>    
    READ-LINE THROW IF       
        DROP -ROT            
        NEXT-NUMBERS
    ELSE
        DROP 2DROP 
    THEN ;

: READ-QUERY-ARGS ( addr,u,2var,fd -- )
    >R -ROT OVER SWAP R> 
    READ-LINE THROW IF   
        DROP SWAP 2      
        NEXT-NUMBERS
    ELSE
        DROP 2DROP
    THEN ;

: PRINT-NUMBERS ( array,u -- )
    0 DO
        DUP I CELLS + @ . 
    LOOP DROP ;


: FIRST-LEAF-POSITION ( size -- pos )
    1 SWAP
    BEGIN
        DUP 0> WHILE
            2/ SWAP 2* SWAP
    REPEAT DROP ;



: BUILD-LEAVES ( tree,array,u -- )
    DUP >R FIRST-LEAF-POSITION 
    CELLS ROT + SWAP           
    R> 0 DO 
        OVER OVER           
        @ SWAP !            
        CELL+ SWAP          
        CELL+ SWAP          
    LOOP 2DROP ;


: BUILD-NODE-LEVEL ( tree,pos -- )
    DUP 2/ DO             
        DUP I 2* CELLS +  
        DUP CELL+         
        @ SWAP @ +        
        OVER I CELLS + !  
    LOOP DROP ;


: BUILD-NODE-LEVELS ( tree,size -- )
    FIRST-LEAF-POSITION      
    BEGIN
        2DUP BUILD-NODE-LEVEL 
        2/                    
    DUP 1 = UNTIL 
    2DROP ;


: BUILD-TREE ( array,tree,size -- )
    OVER !    
    DUP ROT OVER @ 
    BUILD-LEAVES   
    DUP @          
    BUILD-NODE-LEVELS ;


: MIDDLE ( left,right -- middle )
    OVER - 2/ + ; 

: INTERVALS ( left,right -- left,middle,middle+1,right )
    OVER OVER MIDDLE  
    DUP 1+ ROT ;      

: SAME-INTERVALS? ( left,right,x,y -- flag )
    D= ;

: QUERY-NODE ( left,right,x,y,tree,pos -- sum )
    2>R               
    2DUP <= IF
        2OVER 2OVER 
        SAME-INTERVALS? IF
            2DROP 2DROP 2R>
            CELLS + @
        ELSE                  
            2SWAP INTERVALS   
            2>R DUP >R        
            2OVER             
            R>                
            MIN               
            2ROT              
            2R>               
            OVER >R           
            2SWAP             
            SWAP R>           
            MAX SWAP          
            2R@               
            2* 1+             
            RECURSE           
            2R>               
            ROT >R            
            2*                
            RECURSE           
            R> +              
        THEN
    ELSE   
        2DROP 2DROP 2R> 2DROP
        0
    THEN ;

: QUERY-SUM ( x,y,tree -- sum )
    DUP @                    
    FIRST-LEAF-POSITION 1-   
    0 SWAP                   
    ROT >R                   
    2SWAP R> 1               
    QUERY-NODE ;
    
DEFER ACTION


: DO-QUERY ( y,x -- )
    DUP -ROT            
    DO                  
        DUP I           
        ACTION    
    LOOP DROP ;


: DO-QUERIES ( y,x -- )
    SWAP 1+ DUP ROT DO
        DUP I DO-QUERY
    LOOP DROP ;

: PRINT2NUMBERS ( x,y -- )
    SWAP . . CR ;

' PRINT2NUMBERS IS ACTION

: PRINT-QUERY-SUM ( x,y -- )
    TREE QUERY-SUM . CR ;

: RECORD-RESULT-MAX ( x,y -- )
    TREE QUERY-SUM 
    RESULT-MAX @ MAX RESULT-MAX ! ;

' RECORD-RESULT-MAX IS ACTION


: SUM-MAX ( x,y -- n )
    INTEGER-MIN RESULT-MAX !    
    SWAP DO-QUERIES 
    RESULT-MAX @ ;

: PROCESS ( addr,u,fd -- )
    3DUP                 
    READ-NUMBER IF       
        MAX-NUMBER !
        3DUP             
        NUMBERS MAX-NUMBER @ ROT 
        READ-NUMBERS             
        NUMBERS TREE MAX-NUMBER @ 
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

INPUT-LINE LINE-MAX-LENGTH STDIN PROCESS BYE
