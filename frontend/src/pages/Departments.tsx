
import { FormEvent, useEffect, useState } from 'react';
import { Pencil, Plus, Trash2 } from 'lucide-react';
import { api } from '../services/api';
import type { Department } from '../types';

export default function Departments() {
  const [items, setItems] = useState<Department[]>([]);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [editing, setEditing] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const load = async () => {
    try {
      setLoading(true);
      setError('');

      const response = await api.get('/departments');
      setItems(response.data);
    } catch (err: any) {
      console.error('Failed to load departments:', err);
      setError(
        err.response?.data?.message ||
        'Failed to load departments. Please check whether the API is running.'
      );
    } finally {
      setLoading(false);
    }
  };

  // IMPORTANT:
  // Do not use: useEffect(load, [])
  // because load() returns a Promise.
  useEffect(() => {
    const fetchDepartments = async () => {
      await load();
    };

    fetchDepartments();
  }, []);

  const submit = async (e: FormEvent) => {
    e.preventDefault();

    try {
      setError('');

      if (editing !== null) {
        await api.put(`/departments/${editing}`, {
          name,
          description,
        });
      } else {
        await api.post('/departments', {
          name,
          description,
        });
      }

      setName('');
      setDescription('');
      setEditing(null);

      await load();
    } catch (err: any) {
      console.error('Failed to save department:', err);

      setError(
        err.response?.data?.message ||
        'Failed to save department. Please try again.'
      );
    }
  };

  const edit = (department: Department) => {
    setEditing(department.id);
    setName(department.name);
    setDescription(department.description ?? '');
  };

  const cancelEdit = () => {
    setEditing(null);
    setName('');
    setDescription('');
    setError('');
  };

  const remove = async (id: number) => {
    if (!window.confirm('Delete this department?')) {
      return;
    }

    try {
      setError('');

      await api.delete(`/departments/${id}`);

      await load();
    } catch (err: any) {
      console.error('Failed to delete department:', err);

      setError(
        err.response?.data?.message ||
        'Cannot delete this department.'
      );
    }
  };

  return (
    <div>
      <div className="page-head">
        <div>
          <h2>Departments</h2>
          <p>Manage organizational departments.</p>
        </div>
      </div>

      {error && (
        <div className="alert error">
          {error}
        </div>
      )}

      <div className="grid-2">
        <form className="panel" onSubmit={submit}>
          <h3>
            {editing !== null
              ? 'Edit Department'
              : 'Add Department'}
          </h3>

          <label>
            Name
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Enter department name"
              required
            />
          </label>

          <label>
            Description
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Enter department description"
              rows={4}
            />
          </label>

          <div className="form-actions">
            <button
              type="submit"
              className="primary"
              disabled={loading}
            >
              <Plus size={17} />

              {editing !== null
                ? 'Update'
                : 'Add'}
            </button>

            {editing !== null && (
              <button
                type="button"
                className="secondary"
                onClick={cancelEdit}
              >
                Cancel
              </button>
            )}
          </div>
        </form>

        <div className="panel">
          <h3>Department List</h3>

          {loading ? (
            <div className="loading">
              Loading departments...
            </div>
          ) : items.length === 0 ? (
            <div className="empty-state">
              No departments found.
            </div>
          ) : (
            <div className="simple-list">
              {items.map((department) => (
                <div key={department.id}>
                  <div>
                    <strong>{department.name}</strong>

                    <small>
                      {department.description || 'No description'}
                    </small>
                  </div>

                  <div className="actions">
                    <button
                      type="button"
                      onClick={() => edit(department)}
                      title="Edit department"
                    >
                      <Pencil size={16} />
                    </button>

                    <button
                      type="button"
                      onClick={() => remove(department.id)}
                      title="Delete department"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
