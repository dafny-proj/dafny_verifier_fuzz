datatype Color = Red | White | Blue
predicate Below(c: Color, d: Color)
{
  c == Red || c == d || d == Blue
}

method DutchFlag(a: array<Color>)
  modifies a
  ensures forall i, j :: 0 <= i < j < a.Length ==> Below(a[i], a[j])
  ensures multiset(a[..]) == multiset(old(a[..]))
{
  var r, w, b := 0, 0, a.Length;
  while w < b
    invariant 0 <= r <= w <= b <= a.Length
    invariant forall i :: 0 <= i < r ==> a[i] == Red
    invariant forall i :: r <= i < w ==> a[i] == White
    invariant forall i :: b <= i < a.Length ==> a[i] == Blue
    invariant multiset(a[..]) == multiset(old(a[..]))
  {
    match a[w]
    case Red =>
      a[r], a[w] := a[w], a[r];
      r, w := r + 1, w + 1;
    case White =>
      w := w + 1;
    case Blue =>
      a[b - 1], a[w] := a[w], a[b - 1];
      b := b - 1;
  }
}

method SortBooleans(a: array<bool>)
  modifies a
  ensures forall i, j :: 0 <= i < j < a.Length ==> !a[i] || a[j]
  ensures multiset(a[..]) == multiset(old(a[..]))
{
  var l, r := 0, a.Length;
  while l < r
    invariant 0 <= l <= r <= a.Length
    invariant forall i :: 0 <= i < l ==> a[i] == false
    invariant forall i :: r <= i < a.Length ==> a[i] == true
    invariant multiset(a[..]) == multiset(old(a[..]))
  {
    match a[l]
    case false =>
      l := l + 1;
    case true =>
      a[l], a[r - 1] := a[r - 1], a[l];
      r := r - 1;
  }
}