VARIABLE MAX-NUMBER
: make-numbers
    MAX-NUMBER @ 0 DO I . LOOP ;

NEXT-ARG EVALUATE MAX-NUMBER !
MAX-NUMBER @ . CR
make-numbers CR
1 . MAX-NUMBER @ . CR
bye
