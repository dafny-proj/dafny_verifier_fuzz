namespace AST_new;

public class InvalidASTOperationException : Exception {
  public InvalidASTOperationException(string message) : base(message) { }

  public InvalidASTOperationException(object trigger, string operation, string message = "\r")
  : this($"Invalid {operation} for `{trigger.GetType()}`. {message}") { }
}

public class UnsupportedASTOperationException : Exception {
  public UnsupportedASTOperationException(string message) : base(message) { }

  public UnsupportedASTOperationException(object trigger, string operation)
  : this($"Unsupported {operation} for `{trigger.GetType()}`.") { }
}

public class UnsupportedNodeCloningException : UnsupportedASTOperationException {
  public UnsupportedNodeCloningException(Node trigger)
  : base(trigger, "cloning") { }
}

public class UnsupportedNodeChildrenException
: UnsupportedASTOperationException {
  public UnsupportedNodeChildrenException(Node trigger)
  : base(trigger, "children retrieval") { }
}

public class UnsupportedNodeChildReplacementException
: UnsupportedASTOperationException {
  public UnsupportedNodeChildReplacementException(Node trigger)
  : base(trigger, "child replacement") { }
}

public class UnsupportedNodePrintingException
: UnsupportedASTOperationException {
  public UnsupportedNodePrintingException(Node trigger)
  : base(trigger, "printing") { }
}
