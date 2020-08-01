#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#define MAX_NUMBER 100
#define MINUS_INFINITY (-999999999);
int Numbers[MAX_NUMBER];

int random_number(int range) {
    int number = rand() % (range+1);
    return (rand () % 3) ? number : -number;
}

int max(int a, int b) {
    return a > b ? a : b;
}

int segment_sum(int *numbers, int x, int y) {
    int result = 0;
    for (int i=x; i<=y; i++) {
            result += numbers[i-1];
        }
    return result;
}

int max_segment_sum(int *numbers, int x, int y) {
    int result = MINUS_INFINITY;
    for(int i=x; i<=y; i++) 
        for (int j=i; j<=y; j++) 
            result = max(result, segment_sum(numbers, i, j));
    return result;
}

int main(int argc, char *argv[]) {
    int max_number = 10;
    int range = 15007;
    if (argc>1) {
        sscanf(argv[1],"%d",&max_number);
        if(max_number > MAX_NUMBER) {
            fprintf(stderr,"data set too large");
            return -1;
        }
    }
    if (argc>2) {
        sscanf(argv[2],"%d",&range);
    }
    srand(time(0));
    printf("# test case for cyatq -- random generated\n");
    printf("# lines beginning with # are comments\n");
    printf("# lines containing left arrow are part of the test input\n");
    printf("# lines beginning with right arrowi are part of the expected result\n");
    printf("<- %d\n<- ", max_number);
    for(int i=0; i<max_number; i++) {
        Numbers[i] = random_number(range);
        printf("%d ", Numbers[i]);
    }
    putchar('\n');
    int max_queries = (max_number * (max_number + 1)) / 2;
    printf("<- %d\n", max_queries);
    for(int i=1; i<=max_number; i++) {
        for(int j=i; j<=max_number; j++) {
            printf("<- %d %d\n", i, j);
            printf("-> %d\n", max_segment_sum(Numbers, i, j));
        }
    }
    return 0;
}
