#include <stdio.h>
#include <ctype.h>
#include <limits.h>

#define MAXLINE 500000
#define MAXNUMBER 50000

int Numbers[MAXNUMBER];
int Tree[MAXNUMBER*4];
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
int first_leaf_position(int n) {
    int result = 1;
    for (int i=n; i>0; i/=2)
        result *= 2;
    return result;
}

void build_leaves(int *numbers, int n, int f,int *tree) {
    for (int i=0 ; i<n; i++) 
        tree[f+i] = numbers[i];
}

void build_node_level(int l, int *tree) {
    for(int i = l/2; i<l; i++) {
        tree[i] = tree[2*i]+tree[2*i+1];
    }
}

void build_node_levels(int f, int *tree) {
    for(int i = f; i>1; i/=2)
        build_node_level(i, tree);
}
void build_tree(int *numbers, int n, int *tree) {
    tree[0] = n;
    int f = first_leaf_position(n);
    build_leaves(numbers, n, f, tree);
    build_node_levels(f, tree);
}

int min(int a, int b) {
    return a < b ? a : b;
}

int max(int a, int b) {
    return a > b ? a : b;
}

int query_node(int l, int r, int x, int y, int p, int *tree) {
    if (x>y)
        return 0;
    if (x == l && y == r)
        return tree[p];
    int m = l+(r-l)/2;
    return(query_node(l  ,m  ,x         ,min(y,m),p*2,tree) +
           query_node(m+1,r  ,max(x,m+1),y       ,p*2+1,tree));
}
int query_sum(int x, int y, int *tree) {
    int l = 0;
    int r = tree[0]-1;
    int p = first_leaf_position(tree[0]);
    return query_node(l,r,x,y,p,tree);
}
void print_numbers(int *numbers, int n) {
    for (int i=0; i<n; i++) {
        printf("%d ",numbers[i]);
    }
    printf("\n");
}
int main() {
    get_line(Line);
    get_line(Line);
    int n = get_ints(Line, Numbers);
    build_tree(Numbers, n, Tree);
    get_line(Line);
    int query_args[2];
    get_ints(Line, &max_query);
    for(int i=0; i<max_query; i++) {
        get_line(Line);
        get_ints(Line, query_args);
        int acc = INT_MIN;
        for(int i=query_args[0]-1; i<query_args[1]; i++)
            for(int j=i; i<query_args[1]; j++) {
                int s = query_sum(i, j, Tree);
                acc = max(s,acc);
        }
        printf("%d\n",acc);
    }
    return 0;
}
