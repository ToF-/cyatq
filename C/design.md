

    p->sum = p->left->sum + p->right->sum
    p->prefsum = max(p->left->prefsum,     p->left->sum  + p->right->prefsum)
    p->suffsum = max(p->right->suffsum, p->right->sum + p->left->suffsum)
    p->maxsum  = max(max(p->left->maxsum, p->right->maxsum), p->left->suffsum + p->right->prefsum)


    example
    [42,17,-23]

    p=1 range=1..3  sum = 36 prefsum = 59 suffsum = 36 maxsum = 59

    p=2 range=1..2  sum = 59 prefsum=59 suffsum=59 maxsum = 59 

    p=3 range=3..3  sum=-23 prefsum=-23 suffsum=-23 maxsum=-23

    p=4 range=1..1  sum =42 prefsum=42  suffsum=42 maxsum=42

    p=5 range=2..2  sum =17 prefsum=17  suffsum=17 maxsum=17


    query (2,3)

    
    mid = 2 

    tleft = query(p->left,2,2,2,3)
    tright= query(p->right,3,3,2,3)
    t range = [2..3] sum = tleft->sum+tright->sum prefsum = max(tleft->prefsum, tleft->sum+tright->prefsum, suffsum = max(tright->suffsum, tright->sum + tleft->suffsum) maxsum = max(tleft->suffsum+tright->prefsum, max(tleft->maxsum, tright->maxsum)


    t range = [2..3] sum = 36 prefsum = 59 suffsum = 36 maxsum = 59

    Tree q1=query_tree(node2,a,mid,i,j);
    Tree q2=query_tree(node2+1,mid+1,b,i,j);
    t.pre=max(q1.pre,q1.tot+q2.pre);
    t.su=max(q2.su,q2.tot+q1.su);
    t.tot=q1.tot+q2.tot;
    t.bst=max(q1.su+q2.pre,max(q1.bst,q2.bst));
