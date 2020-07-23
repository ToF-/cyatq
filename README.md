# Can you answer these queries

https://www.spoj.com/problems/GSS1/

You are given a sequence A[1], A[2], ..., A[N] . ( |A[i]| ≤ 15007 , 1 ≤ N ≤ 50000 ). A query is defined as follows:

- Query(x,y) = Max { a[i]+a[i+1]+...+a[j] ; x ≤ i ≤ j ≤ y }.

Given _M_ queries, your program must output the results of these queries.

## Input
The first line of the input file contains the integer _N_.

In the second line, _N_ numbers follow.

The third line contains the integer _M_.

_M_ lines follow, where line _i_ contains 2 numbers _xi_ and _yi_.

## Output
Your program should output the results of the _M_ queries, one query per line.

## Example

### Input:
    3
    -1 2 3
    1
    1 2

### Output:
    2
