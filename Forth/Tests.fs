REQUIRE ffl/tst.fs
REQUIRE Cyatq.fs


T{ ." GET-NUMBER reads the next number from a string " CR
    S" 4807 42 17 " DROP 
    GET-NUMBER 4807 ?S DROP }T

CREATE MY-NUMBERS 3 CELLS ALLOT
T{ ." GET-NUMBERS reads N numbers from a string and store them " CR
    S" 4807 42 17 " DROP
    MY-NUMBERS 3 GET-NUMBERS 
    MY-NUMBERS 0 CELLS + @ 4807 ?S
    MY-NUMBERS 1 CELLS + @   42 ?S
    MY-NUMBERS 2 CELLS + @   17 ?S }T
BYE
