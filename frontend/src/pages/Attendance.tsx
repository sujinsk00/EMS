import { FormEvent, useEffect, useState } from "react";
import { api } from "../services/api";
import type {
  Attendance,
  AttendanceStatus,
  Employee,
} from "../types";

export default function Attendance() {
  const today = new Date().toISOString().slice(0, 10);

  const [date, setDate] = useState(today);
  const [employeeId, setEmployeeId] = useState("");
  const [status, setStatus] =
    useState<AttendanceStatus>("Present");
  const [notes, setNotes] = useState("");

  const [items, setItems] = useState<Attendance[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);

  const [loading, setLoading] = useState(false);

  const load = async () => {
    try {
      const response = await api.get("/attendance", {
        params: {
          from: date,
          to: date,
          employeeId: employeeId || undefined,
        },
      });

      setItems(response.data);
    } catch (error) {
      console.error("Failed to load attendance:", error);
    }
  };

  useEffect(() => {
    const loadEmployees = async () => {
      try {
        const response = await api.get("/employees", {
          params: {
            active: true,
          },
        });

        setEmployees(response.data);
      } catch (error) {
        console.error("Failed to load employees:", error);
      }
    };

    loadEmployees();
  }, []);

  useEffect(() => {
    load();
  }, [date, employeeId]);

  const submit = async (e: FormEvent) => {
    e.preventDefault();

    if (!employeeId) {
      alert("Please select an employee.");
      return;
    }

    try {
      setLoading(true);

      await api.post("/attendance", {
        employeeId: Number(employeeId),
        date: date,
        status: status,
        notes: notes.trim() || null,
      });

      alert("Attendance saved successfully.");

      setNotes("");

      await load();
    } catch (error: any) {
      console.error(
        "Attendance save failed:",
        error?.response?.data || error
      );

      const message =
        error?.response?.data?.message ||
        error?.response?.data?.title ||
        "Unable to save attendance.";

      alert(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="page-head">
        <div>
          <h2>Attendance</h2>
          <p>Mark and review daily attendance.</p>
        </div>
      </div>

      <form
        className="panel attendance-form"
        onSubmit={submit}
      >
        <label>
          Date

          <input
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
          />
        </label>

        <label>
          Employee

          <select
            value={employeeId}
            onChange={(e) =>
              setEmployeeId(e.target.value)
            }
            required
          >
            <option value="">
              Select employee
            </option>

            {employees.map((employee) => (
              <option
                key={employee.id}
                value={employee.id}
              >
                {employee.employeeCode} —{" "}
                {employee.firstName}{" "}
                {employee.lastName}
              </option>
            ))}
          </select>
        </label>

        <label>
          Status

          <select
            value={status}
            onChange={(e) =>
              setStatus(
                e.target.value as AttendanceStatus
              )
            }
          >
            <option value="Present">
              Present
            </option>

            <option value="Absent">
              Absent
            </option>

            <option value="Leave">
              Leave
            </option>

            <option value="HalfDay">
              Half Day
            </option>
          </select>
        </label>

        <label>
          Notes

          <input
            value={notes}
            onChange={(e) =>
              setNotes(e.target.value)
            }
            placeholder="Optional note"
          />
        </label>

        <button
          className="primary"
          type="submit"
          disabled={loading}
        >
          {loading
            ? "Saving..."
            : "Save Attendance"}
        </button>
      </form>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Employee</th>
              <th>Date</th>
              <th>Status</th>
              <th>Notes</th>
            </tr>
          </thead>

          <tbody>
            {items.length > 0 ? (
              items.map((attendance) => (
                <tr key={attendance.id}>
                  <td>
                    {attendance.employeeName}
                  </td>

                  <td>
                    {attendance.date}
                  </td>

                  <td>
                    <span className="badge">
                      {attendance.status}
                    </span>
                  </td>

                  <td>
                    {attendance.notes || "-"}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4}>
                  No attendance records for
                  this date.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
