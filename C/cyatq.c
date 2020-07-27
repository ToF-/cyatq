#include <stdio.h>
#include <ctype.h>

#define MAXLINE 500000
#define MAXNUMBER 50000

int Numbers[MAXNUMBER];
char Line[MAXLINE];

char *get_line(char *line) {
    fgets(line, MAXLINE, stdin);
    return line;
}

int get_ints(char *line,int *ints) {
    int in_number = 0;
    int acc;
    int minus;
    int count = 0;

    while(*line && *line != '\n') {
        char c = *line++;
        if(isdigit(c)) {
            if(!in_number) {
                in_number = 1;
                acc = 0;
                minus = 0;
            }
            acc = acc * 10 + (c-'0');
        }else if(c=='-') {
            if(!in_number) {
                in_number = 1;
                acc = 0;
                minus = 1;
            }
        }else{
            if (in_number) {
                in_number = 0;
                if (minus) 
                    acc = -acc;
                *ints = acc;
                count++;
                ints++;
            }
        }
    }
    if (in_number) {
        if (minus) 
            acc = -acc;
        *ints = acc;
        count++;
        ints++;
    }
    return count;
}
int main() {
    get_line(Line);
    int n = get_ints(Line, Numbers);
    for(int i=0; i<n; i++) 
        printf("%d\n", Numbers[i]);
    return 0;
}
