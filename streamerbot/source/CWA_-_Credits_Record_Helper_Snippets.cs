// Helper snippets for logging credits data in existing event actions.

private void AddUniqueCredit(string key, string user)
{
    if (string.IsNullOrWhiteSpace(user)) return;

    user = CleanUser(user);
    string current = GetGlobal(key);
    string check = "," + current.Replace(" ", "").ToLowerInvariant() + ",";
    string cleanUser = user.Replace(" ", "").ToLowerInvariant();

    if (check.Contains("," + cleanUser + ",") || check.Contains(",@" + cleanUser + ",")) return;

    if (string.IsNullOrWhiteSpace(current)) current = user;
    else current += ", " + user;

    CPH.SetGlobalVar(key, current, true);
}

private void AddCreditWithValue(string key, string user, string value)
{
    if (string.IsNullOrWhiteSpace(user)) return;

    user = CleanUser(user);
    value = (value ?? "").Trim();

    string item = string.IsNullOrWhiteSpace(value) ? user : user + " (" + value + ")";
    string current = GetGlobal(key);

    if (string.IsNullOrWhiteSpace(current)) current = item;
    else current += ", " + item;

    CPH.SetGlobalVar(key, current, true);
}

private void AddTopCredit(string key, string user, string amountRaw)
{
    if (string.IsNullOrWhiteSpace(user)) return;

    user = CleanUser(user);
    int amount = 0;
    int.TryParse((amountRaw ?? "0").Replace(",", "").Trim(), out amount);

    string current = GetGlobal(key);
    string[] parts = current.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

    bool found = false;
    string output = "";

    foreach (string part in parts)
    {
        string[] kv = part.Split(new[] { '=' }, 2);
        if (kv.Length != 2) continue;

        string name = kv[0];
        int oldAmount = 0;
        int.TryParse(kv[1], out oldAmount);

        if (name.Equals(user, StringComparison.OrdinalIgnoreCase))
        {
            oldAmount += amount;
            found = true;
        }

        if (output.Length > 0) output += "|";
        output += name + "=" + oldAmount;
    }

    if (!found)
    {
        if (output.Length > 0) output += "|";
        output += user + "=" + amount;
    }

    CPH.SetGlobalVar(key, output, true);
}

private string GetGlobal(string key)
{
    try { return CPH.GetGlobalVar<string>(key, true) ?? ""; }
    catch { return ""; }
}

private string CleanUser(string user)
{
    return (user ?? "").Trim().TrimStart('@');
}
