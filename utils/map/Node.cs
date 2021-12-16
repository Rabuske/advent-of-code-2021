class Node <T> {
    public T Value {get; init;}
    public List<Node<T>> AdjacentNodes {get; init; }

    public Node(T value) {
        Value = value;
        AdjacentNodes = new List<Node<T>>();
    }
}