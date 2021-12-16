class Node <T> {
    public T Value {get; set;}
    public List<Node<T>> AdjacentNodes {get; init; }

    public Node(T value) {
        Value = value;
        AdjacentNodes = new List<Node<T>>();
    }
}