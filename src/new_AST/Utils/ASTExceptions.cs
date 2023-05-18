namespace AST_new;

public class UnsupportedNodeOperationException : Exception {
  public UnsupportedNodeOperationException(string message) : base(message) { }

  public UnsupportedNodeOperationException(Node trigger, string operation)
  : this($"Unsupported {operation} for `{trigger.GetType()}`.") { }
}

public class UnsupportedNodeCloningException : UnsupportedNodeOperationException {
  public UnsupportedNodeCloningException(Node trigger)
  : base(trigger, "cloning") { }
}

public class UnsupportedNodeChildrenException
: UnsupportedNodeOperationException {
  public UnsupportedNodeChildrenException(Node trigger)
  : base(trigger, "children retrieval") { }
}

public class UnsupportedNodeChildReplacementException
: UnsupportedNodeOperationException {
  public UnsupportedNodeChildReplacementException(Node trigger)
  : base(trigger, "child replacement") { }
}

public class UnsupportedNodePrintingException
: UnsupportedNodeOperationException {
  public UnsupportedNodePrintingException(Node trigger)
  : base(trigger, "printing") { }
}
