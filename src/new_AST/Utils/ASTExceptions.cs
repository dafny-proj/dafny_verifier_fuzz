namespace AST_new;

public class InvalidASTOperationException : Exception {
  public InvalidASTOperationException(string message) : base(message) { }

  public InvalidASTOperationException(object trigger, string operation, string message = "\r")
  : this($"Invalid {operation} for `{trigger.GetType()}`. {message}") { }
}

public class ChildNotFoundException : InvalidASTOperationException {
  public ChildNotFoundException(string message)
  : base(message) { }

  public ChildNotFoundException(Node parent, Node child)
  : this($"Could not find `{child}` in `{parent}`.") { }
}

public class ParentNotFoundException : InvalidASTOperationException {
  public ParentNotFoundException(string message)
  : base(message) { }

  public ParentNotFoundException(Node child)
  : this($"Could not find parent for `{child}`.") { }
}

public class ChildReplaceIncompatibleTypeException : InvalidASTOperationException {
  public ChildReplaceIncompatibleTypeException(string message)
  : base(message) { }

  public ChildReplaceIncompatibleTypeException(Node parent, Node child, Node newChild)
  : this($"Cannot replace `{child}` with `{newChild}` in `{parent}`.") { }
}

public class DuplicateParentException : InvalidASTOperationException {
  public DuplicateParentException(string message)
  : base(message) { }

  public DuplicateParentException(Node child, Node parent1, Node parent2)
  : this($"Nodes should only have a single parent. Found multiple parents " +
         $"{{`{parent1}`, `{parent2}`}} for `{child}`.") { }
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
