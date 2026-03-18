import { useApp } from '../context/AppContext'
import {
  AreaChart,
  Area,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts'
import { Building2, TrendingUp, DollarSign, Wrench } from 'lucide-react'

const occupancyData = [
  { month: 'Jul', rate: 82 },
  { month: 'Aug', rate: 85 },
  { month: 'Sep', rate: 88 },
  { month: 'Oct', rate: 86 },
  { month: 'Nov', rate: 90 },
  { month: 'Dec', rate: 92 },
]

const revenueData = [
  { month: 'Jul', collected: 45000, expected: 50000 },
  { month: 'Aug', collected: 48000, expected: 52000 },
  { month: 'Sep', collected: 52000, expected: 55000 },
  { month: 'Oct', collected: 49000, expected: 54000 },
  { month: 'Nov', collected: 55000, expected: 58000 },
  { month: 'Dec', collected: 58000, expected: 60000 },
]

interface StatCardProps {
  title: string
  value: string
  change: string
  changeType: 'positive' | 'negative' | 'neutral'
  icon: React.ReactNode
}

function StatCard({ title, value, change, changeType, icon }: StatCardProps) {
  const changeColors = {
    positive: 'text-green-600 bg-green-50 dark:bg-green-900/30 dark:text-green-400',
    negative: 'text-red-600 bg-red-50 dark:bg-red-900/30 dark:text-red-400',
    neutral: 'text-slate-600 bg-slate-50 dark:bg-slate-800 dark:text-slate-400',
  }

  return (
    <div className="card p-6">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm font-medium text-slate-500 dark:text-slate-400">
            {title}
          </p>
          <p className="text-2xl font-bold text-slate-900 dark:text-white mt-1">
            {value}
          </p>
          <span
            className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium mt-2 ${changeColors[changeType]}`}
          >
            {change}
          </span>
        </div>
        <div className="p-3 bg-primary-50 dark:bg-primary-900/30 rounded-xl">
          {icon}
        </div>
      </div>
    </div>
  )
}

export default function Dashboard() {
  const { t, theme } = useApp()

  const chartColors = {
    primary: '#FF5B3C',
    primaryLight: 'rgba(255, 91, 60, 0.2)',
    secondary: '#94a3b8',
    text: theme === 'dark' ? '#94a3b8' : '#64748b',
    grid: theme === 'dark' ? '#1e293b' : '#e2e8f0',
  }

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard
          title="Total Properties"
          value="1,284"
          change="+12% from last month"
          changeType="positive"
          icon={<Building2 className="text-primary-500" size={24} />}
        />
        <StatCard
          title="Occupancy Rate"
          value="92%"
          change="+2% from last month"
          changeType="positive"
          icon={<TrendingUp className="text-primary-500" size={24} />}
        />
        <StatCard
          title="Monthly Revenue"
          value="$58,000"
          change="+5% from last month"
          changeType="positive"
          icon={<DollarSign className="text-primary-500" size={24} />}
        />
        <StatCard
          title="Maintenance Requests"
          value="23"
          change="-3 from last week"
          changeType="negative"
          icon={<Wrench className="text-primary-500" size={24} />}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="card p-6">
          <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">
            {t('occupancyRate')} Trend
          </h3>
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={occupancyData}>
                <defs>
                  <linearGradient id="occupancyGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor={chartColors.primary} stopOpacity={0.3} />
                    <stop offset="95%" stopColor={chartColors.primary} stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke={chartColors.grid} />
                <XAxis dataKey="month" stroke={chartColors.text} fontSize={12} />
                <YAxis
                  stroke={chartColors.text}
                  fontSize={12}
                  domain={[70, 100]}
                  tickFormatter={value => `${value}%`}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: theme === 'dark' ? '#1e293b' : '#fff',
                    border: 'none',
                    borderRadius: '8px',
                    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                  }}
                  formatter={(value: number) => [`${value}%`, 'Occupancy']}
                />
                <Area
                  type="monotone"
                  dataKey="rate"
                  stroke={chartColors.primary}
                  strokeWidth={2}
                  fill="url(#occupancyGradient)"
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="card p-6">
          <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">
            Monthly Revenue
          </h3>
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={revenueData}>
                <CartesianGrid strokeDasharray="3 3" stroke={chartColors.grid} />
                <XAxis dataKey="month" stroke={chartColors.text} fontSize={12} />
                <YAxis
                  stroke={chartColors.text}
                  fontSize={12}
                  tickFormatter={value => `$${value / 1000}k`}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: theme === 'dark' ? '#1e293b' : '#fff',
                    border: 'none',
                    borderRadius: '8px',
                    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                  }}
                  formatter={(value: number) => [`$${value.toLocaleString()}`, '']}
                />
                <Bar
                  dataKey="expected"
                  fill={chartColors.secondary}
                  radius={[4, 4, 0, 0]}
                  name="Expected"
                />
                <Bar
                  dataKey="collected"
                  fill={chartColors.primary}
                  radius={[4, 4, 0, 0]}
                  name="Collected"
                />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="card p-6">
          <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">
            Recent Activity
          </h3>
          <div className="space-y-4">
            {[
              { action: 'New listing submitted', time: '2 hours ago', type: 'listing' },
              { action: 'Maintenance request completed', time: '4 hours ago', type: 'maintenance' },
              { action: 'New user registered', time: '5 hours ago', type: 'user' },
              { action: 'Property approved', time: '6 hours ago', type: 'approval' },
              { action: 'Payment received', time: '1 day ago', type: 'payment' },
            ].map((activity, idx) => (
              <div
                key={idx}
                className="flex items-center justify-between py-2 border-b border-slate-100 dark:border-slate-800 last:border-0"
              >
                <span className="text-sm text-slate-700 dark:text-slate-300">
                  {activity.action}
                </span>
                <span className="text-xs text-slate-500">{activity.time}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="card p-6 lg:col-span-2">
          <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">
            Quick Stats
          </h3>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {[
              { label: 'Total Regions', value: '14' },
              { label: 'Total Districts', value: '206' },
              { label: 'Total Complexes', value: '128' },
              { label: 'Total Buildings', value: '456' },
              { label: 'Pending Listings', value: '34' },
              { label: 'Active Users', value: '2,847' },
              { label: 'Total Units', value: '8,234' },
              { label: 'Occupied Units', value: '7,575' },
            ].map((stat, idx) => (
              <div
                key={idx}
                className="p-4 bg-slate-50 dark:bg-slate-800/50 rounded-lg text-center"
              >
                <p className="text-2xl font-bold text-slate-900 dark:text-white">
                  {stat.value}
                </p>
                <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  {stat.label}
                </p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}
