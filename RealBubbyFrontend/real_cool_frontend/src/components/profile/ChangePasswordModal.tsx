// // src/components/profile/ChangePasswordModal.tsx
//
// import React, { useEffect } from 'react';
// import InputField from '../common/InputField';
// import useChangePassword from '@/hooks/useChangePassword';
//
// interface ChangePasswordModalProps {
//     isOpen: boolean;
//     onClose: () => void;
// }
//
// const ChangePasswordModal: React.FC<ChangePasswordModalProps> = ({ isOpen, onClose }) => {
//     const {
//         formData,
//         error,
//         validationErrors,
//         success,
//         isLoading,
//         handleChange,
//         handleSubmit,
//         resetForm
//     } = useChangePassword();
//
//     useEffect(() => {
//         if (success) {
//             onClose();
//             resetForm();
//         }
//     }, [success, onClose, resetForm]);
//
//     console.log('ChangePasswordModal rendering with formData:', formData);
//     console.log('ChangePasswordModal rendering with validationErrors:', validationErrors);
//
//     return (
//         <div className={`modal ${isOpen ? 'modal-open' : ''}`}>
//             <div className="modal-box">
//                 <h3 className="font-bold text-lg">Change Password</h3>
//                 {error && <div className="alert alert-error mb-4">{error}</div>}
//                 {success && <div className="alert alert-success mb-4">{success}</div>}
//                 <form onSubmit={handleSubmit}>
//                     <InputField
//                         label="Old Password"
//                         type="password"
//                         name="oldPassword"
//                         value={formData.oldPassword}
//                         onChange={handleChange}
//                         error={validationErrors.CurrentPassword?.[0]}
//                         required
//                     />
//                     <InputField
//                         label="New Password"
//                         type="password"
//                         name="newPassword"
//                         value={formData.newPassword}
//                         onChange={handleChange}
//                         error={validationErrors.NewPassword?.[0]}
//                         required
//                     />
//                     <InputField
//                         label="Confirm New Password"
//                         type="password"
//                         name="confirmPassword"
//                         value={formData.confirmPassword}
//                         onChange={handleChange}
//                         error={validationErrors.ConfirmNewPassword?.[0]}
//                         required
//                     />
//                     <div className="modal-action">
//                         <button type="button" className="btn" onClick={onClose}>Cancel</button>
//                         <button type="submit" className="btn btn-primary" disabled={isLoading}>
//                             {isLoading ? 'Changing...' : 'Change Password'}
//                         </button>
//                     </div>
//                 </form>
//             </div>
//         </div>
//     );
// };
//
// export default ChangePasswordModal;
