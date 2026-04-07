import React, { useState, useEffect } from 'react';
import { productApi, orderApi } from './api/orderApi';

const AdminDashboard = () => {
  const [activeTab, setActiveTab] = useState('products');
  const [products, setProducts] = useState([]);
  const [orders, setOrders] = useState([]);
  const [newProduct, setNewProduct] = useState({ name: '', stock: 0 });

  useEffect(() => {
    refreshData();
  }, [activeTab]);

  const refreshData = async () => {
    if (activeTab === 'products') {
      const res = await productApi.getAll();
      setProducts(res.data);
    } else {
      const res = await orderApi.getAll();
      setOrders(res.data);
    }
  };

  // --- Product CRUD ---
  const handleAddProduct = async () => {
    await productApi.create(newProduct);
    setNewProduct({ name: '', stock: 0 });
    refreshData();
  };

  const handleDeleteProduct = async (id) => {
    if (window.confirm("Delete this product?")) {
      await productApi.delete(id);
      refreshData();
    }
  };

  // --- Order CRUD ---
  const handleDeleteOrder = async (id) => {
    if (window.confirm("Remove this order record?")) {
      await orderApi.delete(id);
      refreshData();
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.tabs}>
        <button onClick={() => setActiveTab('products')} style={activeTab === 'products' ? styles.activeTab : styles.tab}>Products</button>
        <button onClick={() => setActiveTab('orders')} style={activeTab === 'orders' ? styles.activeTab : styles.tab}>Orders</button>
      </div>

      {activeTab === 'products' ? (
        <section>
          <h2>Inventory Management</h2>
          <div style={styles.form}>
            <input type="text" placeholder="Product Name" value={newProduct.name} onChange={(e) => setNewProduct({...newProduct, name: e.target.value})} />
            <input type="number" placeholder="Stock" value={newProduct.stock} onChange={(e) => setNewProduct({...newProduct, stock: parseInt(e.target.value)})} />
            <button onClick={handleAddProduct} style={styles.saveBtn}>Add Product</button>
          </div>
          <table style={styles.table}>
            <thead><tr><th>ID</th><th>Name</th><th>Stock</th><th>Actions</th></tr></thead>
            <tbody>
              {products.map(p => (
                <tr key={p.id}>
                  <td>{p.id}</td>
                  <td>{p.name}</td>
                  <td>{p.stock}</td>
                  <td><button onClick={() => handleDeleteProduct(p.id)} style={styles.delBtn}>Delete</button></td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>
      ) : (
        <section>
          <h2>Order Transaction History</h2>
          <table style={styles.table}>
            <thead><tr><th>Order ID</th><th>Status</th><th>Date</th><th>Actions</th></tr></thead>
            <tbody>
              {orders.map(o => (
                <tr key={o.id}>
                  <td>{o.id}</td>
                  <td style={{color: o.status === 2 ? 'green' : 'orange'}}>{o.status === 2 ? 'SUCCESS' : 'PROCESSING'}</td>
                  <td>{new Date(o.createdAt).toLocaleDateString()}</td>
                  <td><button onClick={() => handleDeleteOrder(o.id)} style={styles.delBtn}>Remove</button></td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>
      )}
    </div>
  );
};

const styles = {
  container: { padding: '2rem', maxWidth: '1000px', margin: '0 auto', fontFamily: 'sans-serif' },
  tabs: { display: 'flex', gap: '10px', marginBottom: '2rem' },
  tab: { padding: '10px 20px', cursor: 'pointer', border: '1px solid #ccc', background: '#f9f9f9' },
  activeTab: { padding: '10px 20px', cursor: 'pointer', border: '1px solid #2c3e50', background: '#2c3e50', color: 'white' },
  table: { width: '100%', borderCollapse: 'collapse', marginTop: '1rem' },
  form: { display: 'flex', gap: '10px', marginBottom: '20px' },
  saveBtn: { background: '#27ae60', color: 'white', border: 'none', padding: '10px' },
  delBtn: { background: '#e74c3c', color: 'white', border: 'none', padding: '5px 10px', borderRadius: '4px', cursor: 'pointer' }
};

export default AdminDashboard;