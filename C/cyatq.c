#include <stdio.h>
#include <ctype.h>
#include <limits.h>
#include <assert.h>
#include <string.h>

#define MAXLINE 500000
#define MAXNUMBER 50000

int Numbers[MAXNUMBER];
int SumTree[MAXNUMBER*4];
int MaxTree[MAXNUMBER*4];
char Line[MAXLINE];

int get_int(char *line) {
    int n;
    fgets(line, MAXLINE, stdin);
    sscanf(line, "%d", &n);
    return n;
}

int get_ints(char *line, int *ints) {
    fgets(line, MAXLINE, stdin);
    char *strToken = strtok(line, " " );
    int n = 0;
    while(strToken != NULL) {
        sscanf(strToken, "%d", ints); 
        n++;
        ints++;
        strToken = strtok(NULL, " ");
    }
    return n;
}

void build_sum_node(int l, int r, int *numbers, int *tree, int p) {
    if (l == r) {
        tree[p] = numbers[l-1];
    } else {
        int m = l + (r-l) / 2;
        build_sum_node(l,   m, numbers, tree, p*2);
        build_sum_node(m+1, r, numbers, tree, p*2+1);
        tree[p] = tree[p*2] + tree[p*2+1];
    }
}

void build_sum_tree(int *numbers, int n, int *tree) {
    build_sum_node(1, n, numbers, tree, 1);
}

int max(int a, int b) {
    return a > b ? a : b;
}

void build_max_sum_node(int l, int r, int *sumTree, int *maxTree, int p) {
    if (l == r) {
        maxTree[p] = sumTree[p];
    }
    else {
        int m = l + (r-l) / 2;
        build_max_sum_node(l,   m, sumTree, maxTree, p*2);
        build_max_sum_node(m+1, r, sumTree, maxTree,p*2+1);
        int a = maxTree[p*2];
        int b = maxTree[p*2+1];
        maxTree[p] = max(a, max(b, a+b));
    }
}

void build_max_sum_tree(int n, int *sumTree,int *maxTree) {
    build_max_sum_node(1, n, sumTree, maxTree, 1);
}
int min(int a, int b) {
    return a < b ? a : b;
}

int query_sum_node(int l, int r, int x, int y, int p, int *tree) {
    if (x>y)
        return 0;
    if (x == l && y == r)
        return tree[p];
    int m = l+(r-l)/2;
    return(query_sum_node(l  ,m  ,x         ,min(y,m),p*2,tree) +
           query_sum_node(m+1,r  ,max(x,m+1),y       ,p*2+1,tree));
}
int query_sum(int x, int y, int *tree, int n) {
    return query_sum_node(1,n,x,y,1,tree);
}
int query_max_sum_node(int l, int r, int x, int y, int p, int *tree) {
    if (x>y)
        return 0;
    if (x == l && y == r)
        return tree[p];
    int m = l+(r-l)/2;
    int a = query_max_sum_node(l  ,m  ,x         ,min(y,m),p*2,tree);
    int b = query_max_sum_node(m+1,r  ,max(x,m+1),y       ,p*2+1,tree);
    return max(a,b);
}
int query_max_sum(int x, int y, int *tree, int n) {
    return query_max_sum_node(1,n,x,y,1,tree);
}

void print_numbers(int *numbers, int n) {
    for (int i=0; i<n; i++) {
        printf("%d ",numbers[i]);
    }
    printf("\n");
}
int main() {
    int max_number = get_int(Line);
    int n = get_ints(Line, Numbers);
    build_sum_tree(Numbers, n, SumTree);
    build_max_sum_tree(n, SumTree,MaxTree);
    int max_query = get_int(Line);
    int query_args[2];
    for(int q=0; q<max_query; q++) {
        get_ints(Line, query_args);
        int x = query_args[0];
        int y = query_args[1];
        int s = query_max_sum(x, y, MaxTree, max_number);
        printf("%d\n",s);
    }
    return 0;
}
