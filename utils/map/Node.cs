class Node <T> {
    public T Value {get; set;}
    public Dictionary<Node<T>, int> AdjacentNodes {get; init; }

    public Node(T value) {
        Value = value;
        AdjacentNodes = new Dictionary<Node<T>, int>();
    }

    public int GetTravelCostTo(Node<T> adjacent) {
        return AdjacentNodes[adjacent] == -1? adjacent.ValueAsInt : AdjacentNodes[adjacent];
    }

    private int ValueAsInt => Convert.ToInt32(Value);
    
}