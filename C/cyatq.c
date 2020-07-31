#include <stdio.h>
#include <ctype.h>
#include <limits.h>
#include <assert.h>
#include <string.h>

#define MAXLINE 500000
#define MAXNUMBER 50000
#define LEFT(p)  (p*2)
#define RIGHT(p) (p*2+1)

int Numbers[MAXNUMBER];
int SumTree[MAXNUMBER*4];
int MaxTree[MAXNUMBER*4];
char Line[MAXLINE];

struct node {
    int segment_sum;
    int max_prefix_sum;
    int max_suffix_sum;
    int max_segment_sum;
} SegmentTree[MAXNUMBER*4];


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

int max(int a, int b) {
    return a > b ? a : b;
}

void build_tree(int l, int r, int *numbers, struct node *tree, int p) {
    if (l == r) {
        int v = numbers[l+1];
        tree[p].segment_sum     = v;
        tree[p].max_prefix_sum  = v;
        tree[p].max_suffix_sum  = v;
        tree[p].max_segment_sum = v;
    } 
    else {
        int m = l + (r - l) / 2;
        build_tree(l, m, numbers, tree, LEFT(p));
        build_tree(m+1,r,numbers, tree, RIGHT(p));
        struct node left  = tree[LEFT(p)];
        struct node right = tree[RIGHT(p)];
        tree[p].segment_sum = left.segment_sum + right.segment_sum;
        tree[p].max_prefix_sum = max(left.max_prefix_sum,  left.segment_sum + right.max_prefix_sum);
        tree[p].max_suffix_sum = max(right.max_suffix_sum, right.max_suffix_sum + left.max_suffix_sum);
        tree[p].max_segment_sum = max(left.max_segment_sum,max(right.max_segment_sum, left.max_suffix_sum + right.max_prefix_sum)); 
    }
}

struct node query_tree(int l, int r, int x, int y, struct node *tree, int p) {
    if (x == l && y == r) {
        return tree[p];
    }
    else {
        int m = l + (r - l) / 2;
        struct node left = query_tree(l, m, x, y, tree, LEFT(p));
        struct node right= query_tree(m+1,r,x, y, tree, RIGHT(p));
        struct node result;
        result.segment_sum = left.segment_sum + right.segment_sum;
        result.max_prefix_sum = max(left.max_prefix_sum, left.segment_sum + right.max_prefix_sum);
        result.max_suffix_sum = max(right.max_suffix_sum, right.segment_sum + left.max_suffix_sum);
        result.max_segment_sum = max(left.max_segment_sum, max(right.max_segment_sum, 
                                     left.max_suffix_sum + right.max_prefix_sum));
        return result;
    }
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
    build_tree(1, n, Numbers, SegmentTree, n);
    int max_query = get_int(Line);
    int query_args[2];
    for(int q=0; q<max_query; q++) {
        get_ints(Line, query_args);
        int x = query_args[0];
        int y = query_args[1];
        struct node result = query_tree(1, n, x, y, SegmentTree, 1);
        printf("%d\n",result.max_segment_sum);
    }
    return 0;
}
