def minPathSum(self, grid: list[list[int]]) -> int:
    memo = [[float("inf") for _ in range(len(grid[0]))]] * len(grid)
    memo[0][0] = grid[0][0]
    for i in range(1, len(grid[0])):
        memo[0][i] = grid[0][i] + memo[0][i - 1]
    for m in range(1, len(grid)):
        for n in range(len(grid[0])):
            if n == 0:
                memo[m][n] == grid[m][n] + memo[m - 1][n]
            else:
                memo[m][n] == grid[m][n] + min(memo[m - 1][n], memo[m][n - 1])
    return memo[-1][-1]
