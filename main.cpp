#include <bits/stdc++.h>
#include <cstdio>
#include <ext/pb_ds/assoc_container.hpp>
#include <ext/pb_ds/tree_policy.hpp>
#include <fstream>
#include <functional>
#include <iomanip>
#include <ios>
#include <queue>
#include <unordered_map>
#include <utility>
#include <vector>
#define Kero                                                                                                           \
    ios_base::sync_with_stdio(0);                                                                                      \
    cout.tie(0);                                                                                                       \
    cin.tie(0)
#define ll long long
#define ull unsigned long long
#define all(x) x.begin(), x.end()
#define allr(x) x.rbegin(), x.rend()
// #define int ll
#define ld long double
#define pii pair<int, int>
// #define endl '\n'
#define see(x) " [" << #x << " = " << (x) << "] "
using namespace std;
using namespace __gnu_pbds;
template <typename T>
using ordered_set = tree<T, null_type, less<T>, rb_tree_tag, tree_order_statistics_node_update>;
template <typename T>
using ordered_multiset = tree<T, null_type, less_equal<T>, rb_tree_tag, tree_order_statistics_node_update>;
// order_of_key (k) : Number of items strictly smaller than k
// find_by_order(k) : K-th element in a set (counting from zero)
const int INF = 1e9, N = 2e5 + 5, mod = 1e9 + 7;
const ld pi = 2 * acos(0), eps = 1e-6;

void fileIO()
{
#ifndef ONLINE_JUDGE
    // freopen("io/input.txt", "r", stdin);
    // freopen("io/output.txt", "w", stdout);
#endif
}

// Coding is so easy if you simulate on paper first
const ld WALK_SPEED = 5.0, inf = numeric_limits<ld>::infinity(); // km/h to m/s
struct point
{
    ld x;
    ld y;
};

ld eDist(point a, point b)
{
    ld diff_x = a.x - b.x;
    ld diff_y = a.y - b.y;
    return sqrtl(diff_x * diff_x + diff_y * diff_y);
}

void solve(ifstream &mp, ifstream &quries)
{
    int n;
    mp >> n;
    vector<point> intersection(n);
    for (int i = 0; i < n; i++)
    {
        int idx;
        mp >> idx >> intersection[idx].x >> intersection[idx].y;
        // cout << idx << ' ' << intersection[idx].x << ' ' << intersection[idx].y << endl;
    }
    int m;
    mp >> m;
    vector<vector<pair<ld, int>>> roads(n);
    unordered_map<int, unordered_map<int, int>> lengths;
    while (m--)
    {
        int u, v;
        mp >> u >> v;
        ld len, spd; // in km
        mp >> len >> spd;
        ld cost = len / spd;
        lengths[u][v] = len;
        lengths[v][u] = len;
        roads[u].push_back({cost, v});
        roads[v].push_back({cost, u});
    }

    int q;
    quries >> q;
    int testNumber = 0;
    while (q--)
    {
        // cout << "Test #" << ++testNumber << " : ";
        point start, end;
        quries >> start.x >> start.y >> end.x >> end.y;
        int r_int;
        quries >> r_int;
        ld r = (ld)r_int / 1000.0;
        vector<ld> dist(n, inf);
        vector<int> par(n, -1);
        priority_queue<pair<ld, int>, vector<pair<ld, int>>, greater<>> pq;
        for (int i = 0; i < n; i++) // O(N lg N)
        {
            ld e_dist = eDist(start, intersection[i]);
            if (e_dist < r or abs(e_dist - r) <= eps)
                dist[i] = e_dist / WALK_SPEED, pq.push({dist[i], i}), par[i] = i;
        }
        while (pq.size()) // O((V + E) log V)
        {
            auto [cost, cur] = pq.top();
            pq.pop();
            if (cost > dist[cur] and abs(dist[cur] - cost) > eps)
                continue;
            for (auto &[ch_cost, ch] : roads[cur])
            {
                if (dist[ch] < (ch_cost + cost) or abs(dist[ch] - ch_cost - cost) <= eps)
                    continue;
                par[ch] = cur;
                dist[ch] = ch_cost + cost;
                pq.push({dist[ch], ch});
            }
        }
        vector<int> path;
        ld ans = inf;
        ld st_end = eDist(start, end);
        if (st_end < r or abs(st_end - r) <= eps)
            ans = st_end / WALK_SPEED;
        int end_inter = -1;
        for (int i = 0; i < n; i++) // O(N)
        {
            ld e_dist = eDist(end, intersection[i]);
            if (e_dist < r or abs(e_dist - r) <= eps)
            {
                ld cur = dist[i] + (e_dist / WALK_SPEED);
                ans = min(cur, ans);
                if (cur == ans)
                {
                    end_inter = i;
                }
            }
        }
        path.push_back(end_inter);
        while (par[end_inter] != end_inter)
        {
            end_inter = par[end_inter];
            path.push_back(end_inter);
        }
        int sz = path.size();
        for (int i = sz - 1; i >= 0; --i)
            cout << path[i] << " \n"[i == 0];
        ld vic_dist = 0;
        for (int i = 1; i < sz; i++)
            vic_dist += lengths[path[i]][path[i - 1]];
        ld walk_dist = eDist(intersection[path[0]], end) + eDist(intersection[path[sz - 1]],start);
        cout << ans * 60 << " mins" << endl;
        cout << vic_dist + walk_dist << " km" << endl;
        cout << walk_dist << " km" << endl;
        cout << vic_dist << " km" << endl;
        cout << endl;
    }
}

int main(int argc, char *argv[])
{
    // Kero;
    // fileIO();
    ifstream mp(argv[1]);
    ifstream quries(argv[2]);
    assert(argc == 3);
    // cout << argv[1] << endl;
    // cout << argv[2] << endl;
    freopen("io/output.txt", "w", stdout);
    cout << fixed << setprecision(2);
    ull t = 1;
    // cin >> t;
    auto start = chrono::high_resolution_clock::now();
    while (t--)
        solve(mp, quries);
    auto stop = chrono::high_resolution_clock::now();
    auto duration = chrono::duration_cast<chrono::milliseconds>(stop - start);
    cout << "0 ms" << endl;
    cout << endl;
    cout << duration.count() << " ms" << endl;
    mp.close();
    quries.close();
    return 0;
}