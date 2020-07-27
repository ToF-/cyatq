
50000 CONSTANT NN
10 NN * CONSTANT LL
CREATE INPUT-LINE LL ALLOT
VARIABLE N
CREATE NS NN CELLS ALLOT
CREATE TREE    NN 4 * CELLS ALLOT
VARIABLE RESULT-MAX
2VARIABLE QA

: 3DUP ( a,b,c -- a,b,c,a,b,c )
    DUP 2OVER ROT ; 
    
HEX -8000000000000000 DECIMAL CONSTANT INTEGER-MIN

: IS-#? ( char -- flag )
    DUP  48 >= 
    SWAP 57 <= AND ;

: >-># ( addr -- addr )
    BEGIN 
        DUP C@          
        DUP IS-#?   
        SWAP 45 = OR
    0= WHILE 
        1+ 
    REPEAT ;

: IS-N? ( addr,u -- flag )
    OVER + >R        
    >-># 
    DUP R@ < IF      
        DUP C@ 45 = IF 1+ THEN
        DUP R> < IF  
             C@ IS-#?  
        ELSE
            0
        THEN
    ELSE
        R> 2DROP 
        0
    THEN ;

: >>-U ( addr -- addr',u )
    0                 
    BEGIN 
        OVER C@       
        DUP IS-#? 
    WHILE 
        48 - SWAP
        10 * + 
        SWAP 1+ SWAP       
    REPEAT 
    DROP ; 

: >>-N ( addr -- addr',n )
    >->#   
    DUP C@           
    [CHAR] - = IF 1+ -1 ELSE 1 
               THEN  
    SWAP             
    >>-U     
    ROT * ;          
    
: >>-NS ( addr,array,u -- )
    0 DO                 
        SWAP >>-N 
        ROT SWAP OVER    
        ! CELL+          
    LOOP 
    2DROP ;


: <<L ( addr,u,fd -- u,flag )
    >R 2DUP 0 FILL R>
    READ-LINE THROW ;

: <<N ( addr,u,fd -- n,#true | #false )
    ROT DUP 2SWAP       
    <<L IF    
        OVER SWAP       
        IS-N? IF   
            >>-N 
            NIP -1 
        ELSE
            DROP 0
        THEN
    ELSE
        2DROP 0
    THEN ;

: <<NS ( addr,u1,array,u2,fd -- )
    >R 2SWAP OVER SWAP R>    
    READ-LINE THROW IF       
        DROP -ROT            
        >>-NS
    ELSE
        DROP 2DROP 
    THEN ;

: <<QA ( addr,u,2var,fd -- )
    >R -ROT OVER SWAP R> 
    READ-LINE THROW IF   
        DROP SWAP 2      
        >>-NS
    ELSE
        DROP 2DROP
    THEN ;

: PRINT-NS ( array,u -- )
    0 DO
        DUP I CELLS + @ . 
    LOOP DROP ;


: FLP ( size -- pos )
    1 SWAP
    BEGIN
        DUP 0> WHILE
            2/ SWAP 2* SWAP
    REPEAT DROP ;



: }LEAVES ( tree,array,u -- )
    DUP >R FLP 
    CELLS ROT + SWAP           
    R> 0 DO 
        OVER OVER           
        @ SWAP !            
        CELL+ SWAP          
        CELL+ SWAP          
    LOOP 2DROP ;


: }NODE-LEVEL ( tree,pos -- )
    DUP 2/ DO             
        DUP I 2* CELLS +  
        DUP CELL+         
        @ SWAP @ +        
        OVER I CELLS + !  
    LOOP DROP ;


: }NODE-LEVELS ( tree,size -- )
    FLP      
    BEGIN
        2DUP }NODE-LEVEL 
        2/                    
    DUP 1 = UNTIL 
    2DROP ;


: }TREE ( array,tree,size -- )
    OVER !    
    DUP ROT OVER @ 
    }LEAVES   
    DUP @          
    }NODE-LEVELS ;


: MIDDLE ( left,right -- middle )
    OVER - 2/ + ; 

: INTERVALS ( left,right -- left,middle,middle+1,right )
    OVER OVER MIDDLE  
    DUP 1+ ROT ;      

: QUERY-NODE ( left,right,x,y,tree,pos -- sum )
    2>R               
    2DUP <= IF
        2OVER 2OVER 
        D= IF
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
    FLP 1-   
    0 SWAP                   
    ROT >R                   
    2SWAP R> 1               
    QUERY-NODE ;
    
: RECORD-RESULT-MAX ( x,y -- )
    TREE QUERY-SUM 
    RESULT-MAX @ MAX RESULT-MAX ! ;


: DO-QUERY ( y,x -- )
    DUP -ROT            
    DO                  
        DUP I           
        RECORD-RESULT-MAX
    LOOP DROP ;


: DO-QUERIES ( y,x -- )
    SWAP    
    1+     
    DUP   
    ROT  
    DO  
        DUP I   
        DO-QUERY
    LOOP DROP ;


: SUM-MAX ( x,y -- n )
    INTEGER-MIN RESULT-MAX !    
    SWAP DO-QUERIES 
    RESULT-MAX @ ;

: PROCESS ( addr,u,fd -- )
    3DUP                 
    <<N IF       
        N !
        3DUP             
        NS N @ ROT 
        <<NS             
        NS TREE N @ 
        }TREE
        3DUP <<N IF
            0 DO
                3DUP QA SWAP <<QA
                QA 2@ 1- SWAP 1- 
                SUM-MAX . CR
            LOOP
            DROP 2DROP
        ELSE
            EXIT
        THEN
    ELSE
        EXIT
    THEN
;

INPUT-LINE LL STDIN PROCESS BYE
