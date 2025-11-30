import React, { useState, useEffect, useCallback } from 'react';
import { weeklySummaryService } from '../services/weeklySummaryService';
import { usersService } from '../services/entityService';
import { WeeklySummaryResponse, ApplicationUser } from '../types';
import { useAuth } from '../contexts/AuthContext';
import './WeeklySummaryPage.css';

const WeeklySummaryPage: React.FC = () => {
  const [summary, setSummary] = useState<WeeklySummaryResponse | null>(null);
  const [users, setUsers] = useState<ApplicationUser[]>([]);
  const [selectedUserId, setSelectedUserId] = useState<string>('');
  const [weekStartDate, setWeekStartDate] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { user } = useAuth();

  // Load users on mount
  useEffect(() => {
    const loadUsers = async () => {
      try {
        const usersData = await usersService.getAll();
        setUsers(usersData);
        
        // Set current user as default
        if (user && usersData.length > 0) {
          const currentUser = usersData.find(
            u => u.userName === user.username || u.email === user.email
          );
          if (currentUser) {
            setSelectedUserId(currentUser.id);
          }
        }
      } catch (err) {
        console.error('Failed to load users', err);
      }
    };
    loadUsers();
  }, [user]);

  const loadSummary = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await weeklySummaryService.getWeeklySummary(
        selectedUserId,
        weekStartDate || undefined
      );
      setSummary(data);
    } catch (err) {
      setError('Failed to load weekly summary');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, [selectedUserId, weekStartDate]);

  // Load summary when user or date changes
  useEffect(() => {
    if (selectedUserId) {
      loadSummary();
    }
  }, [selectedUserId, weekStartDate, loadSummary]);

  const handleUserChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedUserId(e.target.value);
  };

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setWeekStartDate(e.target.value);
  };

  const navigateWeek = (offset: number) => {
    if (summary) {
      const currentDate = new Date(summary.weekStartDate);
      currentDate.setDate(currentDate.getDate() + offset * 7);
      setWeekStartDate(currentDate.toISOString().split('T')[0]);
    }
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      weekday: 'short', 
      month: 'short', 
      day: 'numeric' 
    });
  };

  const getDayOfWeek = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { weekday: 'short' });
  };

  // Get all unique tasks from the summary
  const getAllTasks = (): string[] => {
    if (!summary) return [];
    const taskNames = new Set<string>();
    summary.dailySummaries.forEach(day => {
      day.taskHours.forEach(th => taskNames.add(th.taskName));
    });
    return Array.from(taskNames).sort();
  };

  const getHoursForTaskOnDay = (dayIndex: number, taskName: string): number => {
    if (!summary) return 0;
    const day = summary.dailySummaries[dayIndex];
    const taskHour = day.taskHours.find(th => th.taskName === taskName);
    return taskHour ? Number(taskHour.hours) : 0;
  };

  const getTaskWeekTotal = (taskName: string): number => {
    if (!summary) return 0;
    const taskTotal = summary.taskWeeklyTotals.find(t => t.taskName === taskName);
    return taskTotal ? Number(taskTotal.hours) : 0;
  };

  if (loading && !summary) {
    return <div className="weekly-summary-loading">Loading...</div>;
  }

  return (
    <div className="weekly-summary-page">
      <div className="weekly-summary-header">
        <h1>Weekly User Hours Summary</h1>
        
        <div className="weekly-summary-controls">
          <div className="control-group">
            <label htmlFor="user-select">User:</label>
            <select
              id="user-select"
              value={selectedUserId}
              onChange={handleUserChange}
              className="user-select"
            >
              <option value="">Select a user</option>
              {users.map(u => (
                <option key={u.id} value={u.id}>
                  {u.userName || u.email}
                </option>
              ))}
            </select>
          </div>

          <div className="control-group">
            <label htmlFor="week-start">Week Starting:</label>
            <input
              id="week-start"
              type="date"
              value={weekStartDate}
              onChange={handleDateChange}
              className="date-input"
            />
          </div>

          <div className="week-navigation">
            <button onClick={() => navigateWeek(-1)} className="nav-button">
              ← Previous Week
            </button>
            <button onClick={() => navigateWeek(1)} className="nav-button">
              Next Week →
            </button>
          </div>
        </div>
      </div>

      {error && <div className="error-message">{error}</div>}

      {summary && (
        <div className="summary-content">
          <div className="week-info">
            <h2>
              Week of {formatDate(summary.weekStartDate)} - {formatDate(summary.weekEndDate)}
            </h2>
          </div>

          <div className="summary-table-container">
            <table className="summary-table">
              <thead>
                <tr>
                  <th className="task-column">Task</th>
                  {summary.dailySummaries.map((day, index) => (
                    <th key={index} className="day-column">
                      {getDayOfWeek(day.date)}
                      <br />
                      <span className="date-small">
                        {new Date(day.date).getDate()}
                      </span>
                    </th>
                  ))}
                  <th className="total-column">Week Total</th>
                </tr>
              </thead>
              <tbody>
                {getAllTasks().map((taskName, taskIndex) => (
                  <tr key={taskIndex}>
                    <td className="task-name">{taskName}</td>
                    {summary.dailySummaries.map((_, dayIndex) => {
                      const hours = getHoursForTaskOnDay(dayIndex, taskName);
                      return (
                        <td key={dayIndex} className="hours-cell">
                          {hours > 0 ? hours.toFixed(2) : '-'}
                        </td>
                      );
                    })}
                    <td className="total-cell">
                      {getTaskWeekTotal(taskName).toFixed(2)}
                    </td>
                  </tr>
                ))}
                <tr className="totals-row">
                  <td className="task-name"><strong>Daily Total</strong></td>
                  {summary.dailySummaries.map((day, index) => (
                    <td key={index} className="total-cell">
                      <strong>{Number(day.totalHours).toFixed(2)}</strong>
                    </td>
                  ))}
                  <td className="grand-total">
                    <strong>{Number(summary.weekTotalHours).toFixed(2)}</strong>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {!summary && !loading && selectedUserId && (
        <div className="no-data">No data available for the selected week.</div>
      )}

      {!selectedUserId && !loading && (
        <div className="no-data">Please select a user to view their weekly summary.</div>
      )}
    </div>
  );
};

export default WeeklySummaryPage;
