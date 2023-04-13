method Double(x: int) returns (r: int)
  requires 0 <= x
  ensures r >= 2 * x
{
  r := x + x;
}

method Triple(x: int) returns (r: int)
  ensures r >= 3 * x
{
  if 0 <= x {
    var y := Double(x);
    r := x + y;
  } else {
    var y := Double(-x);
    r := x + y;
  }
}

method SumMax(x: int, y: int) returns (s: int, m: int)
  ensures s == x + y
  ensures x <= m && y <= m
  ensures m == x || m == y
{
  s := x + y;
  if x < y {
    m := y;
  } else {
    m := x;
  }
}

method SumMaxBackwards(s: int, m: int) returns (x: int, y: int)
  requires m >= s - m
  ensures s == x + y
  ensures x <= m && y <= m
  ensures m == x || m == y
{
  x := m;
  y := s - m;
}